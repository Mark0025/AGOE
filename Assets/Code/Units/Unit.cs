using AGOE.Commands;

namespace AGOE.Units
{
    /// <summary>
    /// Represents a game unit (villager, soldier, etc.).
    /// Plain C# class - no Unity dependencies for testability.
    /// MonoBehaviour wrapper will be created later for Unity integration.
    /// </summary>
    public class Unit : ISelectable
    {
        public int Id { get; }
        public bool IsSelected { get; set; }
        public bool CanBeSelected => Health > 0 && State != UnitState.Dead;

        /// <summary>
        /// Unit's current position in world space.
        /// </summary>
        public Vector3Placeholder Position { get; set; }

        /// <summary>
        /// Unit's current state (Idle, Moving, Attacking, etc.).
        /// </summary>
        public UnitState State { get; set; }

        /// <summary>
        /// Current health points. Unit dies when this reaches 0.
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        /// Maximum health points.
        /// </summary>
        public float MaxHealth { get; }

        /// <summary>
        /// Unit type identifier (e.g., "Villager", "Soldier").
        /// Used for spawning, UI display, and game logic.
        /// </summary>
        public string UnitType { get; }

        /// <summary>
        /// Which player owns this unit (0 = player, 1+ = AI opponents).
        /// </summary>
        public int PlayerId { get; }

        public Unit(int id, string unitType, int playerId, float maxHealth = 100f)
        {
            Id = id;
            UnitType = unitType;
            PlayerId = playerId;
            MaxHealth = maxHealth;
            Health = maxHealth;
            State = UnitState.Idle;
            Position = new Vector3Placeholder(0, 0, 0);
            IsSelected = false;
        }

        /// <summary>
        /// Apply damage to this unit.
        /// Returns true if the unit died from this damage.
        /// </summary>
        public bool TakeDamage(float damage)
        {
            if (State == UnitState.Dead)
                return false;

            Health -= damage;

            if (Health <= 0)
            {
                Health = 0;
                State = UnitState.Dead;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Heal this unit by the specified amount.
        /// Cannot exceed MaxHealth.
        /// </summary>
        public void Heal(float amount)
        {
            if (State == UnitState.Dead)
                return;

            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;
        }

        public override string ToString()
        {
            return $"Unit {Id} ({UnitType}) - HP: {Health}/{MaxHealth} - State: {State}";
        }
    }
}
