namespace AGOE.Units
{
    /// <summary>
    /// Interface for any game entity that can be selected by the player.
    /// Implemented by units, buildings, and potentially other interactive objects.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Unique identifier for this selectable entity.
        /// Used by the selection system to track selected entities.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Whether this entity is currently selected.
        /// Updated by the SelectionSystem.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Whether this entity can currently be selected.
        /// Useful for units that are dead, hidden, or temporarily unavailable.
        /// </summary>
        bool CanBeSelected { get; }
    }
}
