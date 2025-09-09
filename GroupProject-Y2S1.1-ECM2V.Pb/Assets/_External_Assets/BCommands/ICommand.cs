namespace BCommands
{
    /// <summary>
    /// Basic command interface with parameterless Execute.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();
    }
}