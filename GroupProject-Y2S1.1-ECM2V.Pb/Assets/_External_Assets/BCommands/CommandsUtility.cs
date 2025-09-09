using System;
using System.Reflection;

namespace BCommands
{
    public class CommandsUtility
    {
        public static MethodInfo FindMethod(Type targetType, string methodName, params object[] parameters)
        {
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                if (method.Name == methodName && CompareParameters(method, parameters))
                    return method;
            }

            return null;
        }

        public static void ValidateParameterTypes(MethodInfo methodInfo, object[] parameters)
        {
            var methodParams = methodInfo.GetParameters();

            if (methodParams.Length != parameters.Length)
                throw new ArgumentException($"Expected {methodParams.Length} parameters but got {parameters.Length}");

            for (int i = 0; i < methodParams.Length; i++)
            {
                var expected = methodParams[i].ParameterType;
                var actual = parameters[i]?.GetType();

                if (actual != null && !expected.IsAssignableFrom(actual))
                {
                    throw new ArgumentException(
                        $"Parameter {i + 1} type mismatch. Expected: {expected}, Got: {actual}");
                }
            }
        }

        private static bool CompareParameters(MethodInfo methodInfo, object[] parameters)
        {
            var methodParams = methodInfo.GetParameters();
            
            if (methodParams.Length != parameters.Length)
                return false;

            for (int i = 0; i < methodParams.Length; i++)
            {
                var expected = methodParams[i].ParameterType;
                var actual = parameters[i]?.GetType();

                if (actual == null || expected.IsAssignableFrom(actual) && !ReferenceEquals(expected, actual))
                {
                    return false;
                }
            }

            return true;
        }

        public static object[] CombineParameters(object[] dynamicParams, object[] fixedParams)
        {
            var result = new object[dynamicParams.Length + fixedParams.Length];
            Array.Copy(dynamicParams, 0, result, 0, dynamicParams.Length);
            Array.Copy(fixedParams, 0, result, dynamicParams.Length, fixedParams.Length);
            return result;
        }
    }
}