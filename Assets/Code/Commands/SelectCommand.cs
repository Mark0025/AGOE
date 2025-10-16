using System.Collections.Generic;

namespace AGOE.Commands
{
    /// <summary>
    /// Command to select units/buildings.
    /// Can represent single selection, multi-selection, or area selection (marquee).
    /// </summary>
    public class SelectCommand : ICommand
    {
        public string CommandType => "Select";
        public float Timestamp { get; }

        /// <summary>
        /// IDs of entities to select.
        /// </summary>
        public List<int> EntityIds { get; }

        /// <summary>
        /// Selection mode (single, add, replace).
        /// </summary>
        public SelectionMode Mode { get; }

        public SelectCommand(List<int> entityIds, SelectionMode mode, float timestamp)
        {
            EntityIds = entityIds ?? new List<int>();
            Mode = mode;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Convenience constructor for single selection.
        /// </summary>
        public SelectCommand(int entityId, SelectionMode mode, float timestamp)
            : this(new List<int> { entityId }, mode, timestamp)
        {
        }
    }

    /// <summary>
    /// How the selection should be applied.
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// Replace current selection (normal click).
        /// </summary>
        Replace,

        /// <summary>
        /// Add to current selection (shift + click).
        /// </summary>
        Add,

        /// <summary>
        /// Toggle selection state (ctrl + click).
        /// </summary>
        Toggle
    }
}
