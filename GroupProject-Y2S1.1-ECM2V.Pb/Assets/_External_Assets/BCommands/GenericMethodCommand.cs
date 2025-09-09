using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BCommands
{
    /// <summary>
    /// Command implementation that invokes a method on a target object
    /// with fixed parameters set at creation.
    /// </summary>
    public class GenericMethodCommand : ICommand
    {
        private static readonly ConcurrentDictionary<string, MethodInfo> MethodCache = new();
        
        private readonly object _target;
        private readonly string _methodName;
        private readonly object[] _parameters;
        private readonly Type _targetType;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMethodCommand"/> class.
        /// </summary>
        /// <param name="target">Target object instance.</param>
        /// <param name="methodName">Method name to invoke.</param>
        /// <param name="parameters">Parameters to use when invoking.</param>
        public GenericMethodCommand(object target, string methodName, params object[] parameters)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _methodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            _parameters = parameters ?? Array.Empty<object>();
            _targetType = _target.GetType();
            
        }

        public string GetKey()
        {
            StringBuilder sb = new StringBuilder();
            
            if (_target is UnityEngine.Object unityObject)
            {
                sb.Append($"{_targetType.FullName}.{unityObject.GetInstanceID()}.{_methodName}.{_parameters.Length}:");
            }
            else
            {
                // Fallback for non-Unity objects
                sb.Append($"{_targetType.FullName}.{_methodName}.{_parameters.Length}:");
            }
            
            foreach (var parameter in _parameters)
            {
                sb.Append(parameter.GetType());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Executes the method with fixed parameters.
        /// </summary>
        public void Execute()
        {
            
            if (!MethodCache.TryGetValue(GetKey(), out var methodInfo))
            {
                methodInfo = CommandsUtility.FindMethod(_targetType, _methodName, _parameters);
                if (methodInfo == null)
                    throw new MissingMethodException(
                        $"{_targetType.FullName} does not contain method '{_methodName}' with {_parameters.Length} parameters");

                MethodCache[GetKey()] = methodInfo;
            }

            CommandsUtility.ValidateParameterTypes(methodInfo, _parameters);
            
            methodInfo.Invoke(_target, _parameters);
        }

        public static bool operator ==(GenericMethodCommand a, GenericMethodCommand b)
        {
            return a.GetKey() == b.GetKey();
        }

        public static bool operator !=(GenericMethodCommand a, GenericMethodCommand b)
        {
            return !(a.GetKey() == b.GetKey());
        }

        public override bool Equals(object obj)
        {
            if (obj is GenericMethodCommand o)
                return this.GetKey() == o.GetKey();
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetKey());
        }
    }
}