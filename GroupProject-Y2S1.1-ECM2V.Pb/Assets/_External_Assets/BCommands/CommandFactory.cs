namespace BCommands
{
    public class CommandFactory
    {
        public static ICommand GenericCommand(object target, string methodName, params object[] parameters)
        {
            return new GenericMethodCommand(target, methodName, parameters);
        }
        public static ICommand MixedCommand(object target, string methodName, params object[] parameters)
        {
            return new MixedCommand(target, methodName, parameters);
        }
        public static ICommand DynamicCommand(object target, string methodName)
        {
            return new DynamicParameterCommand(target, methodName);
        }
    }
}