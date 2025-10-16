namespace AGOE.Commands
{
    /// <summary>
    /// Base interface for all commands in the game.
    /// Commands represent user actions and are routed through the CommandBus.
    /// This enables decoupling, testing, and potential replay functionality.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Unique identifier for this command type.
        /// Used by the CommandBus for routing to appropriate handlers.
        /// </summary>
        string CommandType { get; }

        /// <summary>
        /// Timestamp when the command was created.
        /// Useful for replay systems and debugging.
        /// </summary>
        float Timestamp { get; }
    }
}
