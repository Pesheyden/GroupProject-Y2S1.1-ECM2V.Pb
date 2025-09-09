using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BCommands
{
    /// <summary>
    /// Command implementation that invokes a method on a target object,
    /// taking parameters dynamically at execution time.
    /// </summary>
    public class DynamicParameterCommand : IDynamicCommand
    {
        private static readonly ConcurrentDictionary<string, MethodInfo> MethodCache = new();

        private readonly string _key;
        private readonly object _target;
        private readonly string _methodName;
        private readonly Type _targetType;
        public Type[] ExpectedParameterTypes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicParameterCommand"/> class.
        /// </summary>
        /// <param name="target">Target object instance.</param>
        /// <param name="methodName">Method name to invoke.</param>
        /// <summary>
        /// Expected types of all dynamic parameters.
        /// </summary>


        public DynamicParameterCommand(object target, string methodName)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _methodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            _targetType = _target.GetType();

            var methodInfo = _targetType.GetMethod(_methodName,BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null)
                throw new ArgumentException($"Method '{_methodName}' not found on type '{_targetType.FullName}'.");

            // Get all parameters of the method (all dynamic in this case)
            var methodParams = methodInfo.GetParameters();

            ExpectedParameterTypes = methodParams.Select(p => p.ParameterType).ToArray();

            _key = GetKey();
        }

        public string GetKey()
        {
            if (_key != null)
                return _key;
            
            StringBuilder sb = new StringBuilder();
            
            string a = $"{_targetType.FullName}.{_methodName}.{ExpectedParameterTypes.Length}:";
            
            sb.Append(a);
            foreach (var type in ExpectedParameterTypes)
            {
                sb.Append(type);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Executes the method with parameters provided at invocation time.
        /// Performs method lookup and parameter validation on each call.
        /// </summary>
        /// <param name="parameters">Parameters to pass to the method.</param>
        public void Execute(params object[] parameters)
        {
            if (!MethodCache.TryGetValue(GetKey(), out var methodInfo))
            {
                methodInfo = CommandsUtility.FindMethod(_targetType, _methodName, parameters);
                if (methodInfo == null)
                    throw new MissingMethodException(
                        $"{_targetType.FullName} does not contain method '{_methodName}' with {parameters.Length} parameters");

                MethodCache[GetKey()] = methodInfo;
            }

            CommandsUtility.ValidateParameterTypes(methodInfo, parameters);

            methodInfo.Invoke(_target, parameters);
        }

        /// <summary>
        /// Implements parameterless Execute from ICommand.
        /// Calls Execute with empty parameter array.
        /// </summary>
        public void Execute()
        {
            Execute(Array.Empty<object>());
        }
        public static bool operator ==(DynamicParameterCommand a, DynamicParameterCommand b)
        {
            return a.GetKey() == b.GetKey();
        }

        public static bool operator !=(DynamicParameterCommand a, DynamicParameterCommand b)
        {
            return !(a.GetKey() == b.GetKey());
        }

        public override bool Equals(object obj)
        {
            if (obj is DynamicParameterCommand o)
                return this.GetKey() == o.GetKey();
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetKey());
        }
    }
}
