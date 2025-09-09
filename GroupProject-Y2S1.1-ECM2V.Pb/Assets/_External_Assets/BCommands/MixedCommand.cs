using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BCommands
{
    /// <summary>
    /// Command that invokes a single method with parameters consisting
    /// of dynamic parameters passed at execution plus fixed parameters set at creation.
    /// </summary>
    public class MixedCommand : ICommand
    {
        private static readonly ConcurrentDictionary<string, MethodInfo> MethodCache = new();

        private readonly string _key;
        private readonly object _target;
        private readonly string _methodName;
        private readonly object[] _fixedParameters;
        private readonly Type _targetType;
        public Type[] ExpectedParameterTypes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedCommand"/> class.
        /// </summary>
        /// <param name="target">Target object to invoke method on.</param>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="fixedParameters">Parameters fixed at creation.</param>
        /// <summary>
        /// Expected types of the dynamic parameters (i.e., method parameters not covered by fixedParameters).
        /// </summary>


        public MixedCommand(object target, string methodName, params object[] fixedParameters)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _methodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            _fixedParameters = fixedParameters ?? Array.Empty<object>();
            _targetType = _target.GetType();

            // Get the MethodInfo for the given methodName
            var methodInfo = _targetType.GetMethod(_methodName,BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null)
                throw new ArgumentException($"Method '{_methodName}' not found on type '{_targetType.FullName}'.");

            // Get all parameters of the method
            var methodParams = methodInfo.GetParameters();

            // Number of fixed parameters provided
            int fixedCount = _fixedParameters.Length;

            // The dynamic parameters are those NOT in the fixedParameters, so they are
            // the first N parameters (methodParams.Length - fixedCount)
            int dynamicCount = methodParams.Length - fixedCount;
            if(dynamicCount < 0)
                throw new ArgumentException("More fixed parameters provided than method parameters.");

            ExpectedParameterTypes = methodParams.Select(p => p.ParameterType).ToArray();
            
            _key = GetKey();
        }

        public string GetKey()
        {
            if (_key != null)
                return _key;
            
            var combinedParams = CommandsUtility.CombineParameters(ExpectedParameterTypes, _fixedParameters);

            StringBuilder sb = new StringBuilder();
            
            var methodKey = $"{_targetType.FullName}.{_methodName}.{combinedParams.Length}:";
            
            sb.Append(methodKey);
            foreach (var parameter in combinedParams)
            {
                sb.Append(parameter.GetType());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Executes the method by combining dynamic parameters passed at runtime
        /// with fixed parameters stored in this command.
        /// </summary>
        /// <param name="dynamicParameters">Parameters provided dynamically at execution.</param>
        public void Execute(params object[] dynamicParameters)
        {
            var combinedParams = CommandsUtility.CombineParameters(dynamicParameters, _fixedParameters);

            if (!MethodCache.TryGetValue(GetKey(), out var methodInfo))
            {
                methodInfo = CommandsUtility.FindMethod(_targetType, _methodName, combinedParams);
                if (methodInfo == null)
                    throw new MissingMethodException(
                        $"{_targetType.FullName} does not contain method '{_methodName}' with {combinedParams.Length} parameters");

                MethodCache[GetKey()] = methodInfo;
            }

            CommandsUtility.ValidateParameterTypes(methodInfo, combinedParams);

            methodInfo.Invoke(_target, combinedParams);
        }

        /// <summary>
        /// Executes the method with only fixed parameters (no dynamic).
        /// </summary>
        public void Execute()
        {
            Execute(Array.Empty<object>());
        }


        
        public static bool operator ==(MixedCommand a, MixedCommand b)
        {
            return a.GetKey() == b.GetKey();
        }

        public static bool operator !=(MixedCommand a, MixedCommand b)
        {
            return !(a.GetKey() == b.GetKey());
        }

        public override bool Equals(object obj)
        {
            if (obj is MixedCommand o)
                return this.GetKey() == o.GetKey();
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetKey());
        }
    }
}
