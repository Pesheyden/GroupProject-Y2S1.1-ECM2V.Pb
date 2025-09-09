namespace BCommands
{
    /// <summary>
    /// Extended command interface supporting dynamic parameters on execution.
    /// </summary>
    public interface IDynamicCommand : ICommand
    {
        /// <summary>
        /// Executes the command with parameters provided at invocation time.
        /// </summary>
        /// <param name="parameters">Parameters to pass to the method.</param>
        void Execute(params object[] parameters);
    }
}