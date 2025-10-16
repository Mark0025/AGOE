namespace AGOE.Units
{
    /// <summary>
    /// Represents the current state of a unit.
    /// Used for state machine logic and AI decision-making.
    /// </summary>
    public enum UnitState
    {
        /// <summary>
        /// Unit is doing nothing, waiting for commands.
        /// </summary>
        Idle,

        /// <summary>
        /// Unit is moving to a target position.
        /// </summary>
        Moving,

        /// <summary>
        /// Unit is gathering resources (wood, food, etc.).
        /// </summary>
        Gathering,

        /// <summary>
        /// Unit is attacking an enemy.
        /// </summary>
        Attacking,

        /// <summary>
        /// Unit is dead and should be removed from the game.
        /// </summary>
        Dead
    }
}
