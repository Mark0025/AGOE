using System.Collections.Generic;

namespace AGOE.Commands
{
    /// <summary>
    /// Command to move units to a target position.
    /// Issued when player right-clicks on terrain with units selected.
    /// </summary>
    public class MoveCommand : ICommand
    {
        public string CommandType => "Move";
        public float Timestamp { get; }

        /// <summary>
        /// Target position in world space (Unity Vector3 will be used in Unity integration)
        /// For now, using a simple struct placeholder
        /// </summary>
        public Vector3Placeholder TargetPosition { get; }

        /// <summary>
        /// IDs of units that should execute this move command
        /// </summary>
        public List<int> UnitIds { get; }

        public MoveCommand(Vector3Placeholder targetPosition, List<int> unitIds, float timestamp)
        {
            TargetPosition = targetPosition;
            UnitIds = unitIds ?? new List<int>();
            Timestamp = timestamp;
        }
    }

    /// <summary>
    /// Placeholder for Unity's Vector3 until Unity is integrated.
    /// Will be replaced with UnityEngine.Vector3
    /// </summary>
    public struct Vector3Placeholder
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vector3Placeholder(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}
