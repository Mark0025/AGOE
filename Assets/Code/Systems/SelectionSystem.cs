using System;
using System.Collections.Generic;
using System.Linq;
using AGOE.Units;

namespace AGOE.Systems
{
    /// <summary>
    /// Manages the current selection of units and buildings.
    /// Supports single selection, multi-selection, and selection filtering.
    /// Decoupled from Unity's input system for testability.
    /// </summary>
    public class SelectionSystem
    {
        private readonly HashSet<ISelectable> _selectedEntities;
        private readonly bool _enableLogging;

        /// <summary>
        /// Event fired when the selection changes (entities added or removed).
        /// Useful for updating UI, highlighting units, etc.
        /// </summary>
        public event Action<IReadOnlyCollection<ISelectable>> OnSelectionChanged;

        /// <summary>
        /// Get all currently selected entities (read-only).
        /// </summary>
        public IReadOnlyCollection<ISelectable> SelectedEntities => _selectedEntities.ToList().AsReadOnly();

        /// <summary>
        /// Number of currently selected entities.
        /// </summary>
        public int SelectionCount => _selectedEntities.Count;

        /// <summary>
        /// Whether any entities are currently selected.
        /// </summary>
        public bool HasSelection => _selectedEntities.Count > 0;

        public SelectionSystem(bool enableLogging = false)
        {
            _selectedEntities = new HashSet<ISelectable>();
            _enableLogging = enableLogging;
        }

        /// <summary>
        /// Select a single entity, clearing any previous selection.
        /// </summary>
        public void SelectSingle(ISelectable entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!entity.CanBeSelected)
            {
                if (_enableLogging)
                    Log($"Cannot select entity {entity.Id} - CanBeSelected is false");
                return;
            }

            // Clear previous selection
            ClearSelection();

            // Add new selection
            _selectedEntities.Add(entity);
            entity.IsSelected = true;

            if (_enableLogging)
                Log($"Selected entity {entity.Id}");

            OnSelectionChanged?.Invoke(SelectedEntities);
        }

        /// <summary>
        /// Add an entity to the current selection without clearing existing selection.
        /// Used for multi-selection (shift-clicking, marquee selection, etc.).
        /// </summary>
        public void AddToSelection(ISelectable entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!entity.CanBeSelected)
            {
                if (_enableLogging)
                    Log($"Cannot add entity {entity.Id} to selection - CanBeSelected is false");
                return;
            }

            if (_selectedEntities.Add(entity))
            {
                entity.IsSelected = true;

                if (_enableLogging)
                    Log($"Added entity {entity.Id} to selection (total: {_selectedEntities.Count})");

                OnSelectionChanged?.Invoke(SelectedEntities);
            }
        }

        /// <summary>
        /// Add multiple entities to the selection at once.
        /// Useful for marquee selection box.
        /// </summary>
        public void AddMultipleToSelection(IEnumerable<ISelectable> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var addedAny = false;

            foreach (var entity in entities)
            {
                if (entity != null && entity.CanBeSelected && _selectedEntities.Add(entity))
                {
                    entity.IsSelected = true;
                    addedAny = true;
                }
            }

            if (addedAny)
            {
                if (_enableLogging)
                    Log($"Added multiple entities to selection (total: {_selectedEntities.Count})");

                OnSelectionChanged?.Invoke(SelectedEntities);
            }
        }

        /// <summary>
        /// Remove an entity from the current selection.
        /// </summary>
        public void RemoveFromSelection(ISelectable entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_selectedEntities.Remove(entity))
            {
                entity.IsSelected = false;

                if (_enableLogging)
                    Log($"Removed entity {entity.Id} from selection (remaining: {_selectedEntities.Count})");

                OnSelectionChanged?.Invoke(SelectedEntities);
            }
        }

        /// <summary>
        /// Clear all selected entities.
        /// </summary>
        public void ClearSelection()
        {
            if (_selectedEntities.Count == 0)
                return;

            foreach (var entity in _selectedEntities)
            {
                entity.IsSelected = false;
            }

            _selectedEntities.Clear();

            if (_enableLogging)
                Log("Selection cleared");

            OnSelectionChanged?.Invoke(SelectedEntities);
        }

        /// <summary>
        /// Check if a specific entity is currently selected.
        /// </summary>
        public bool IsSelected(ISelectable entity)
        {
            if (entity == null)
                return false;

            return _selectedEntities.Contains(entity);
        }

        /// <summary>
        /// Get all selected entities of a specific type.
        /// Useful for filtering units vs buildings, or specific unit types.
        /// </summary>
        public IReadOnlyCollection<T> GetSelectedOfType<T>() where T : ISelectable
        {
            return _selectedEntities.OfType<T>().ToList().AsReadOnly();
        }

        /// <summary>
        /// Get all selected units (assumes Unit implements ISelectable).
        /// </summary>
        public IReadOnlyCollection<Unit> GetSelectedUnits()
        {
            return GetSelectedOfType<Unit>();
        }

        /// <summary>
        /// Select all entities from a given collection.
        /// Useful for "select all units" hotkey.
        /// </summary>
        public void SelectAll(IEnumerable<ISelectable> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            ClearSelection();
            AddMultipleToSelection(entities);
        }

        // Placeholder logging method (will use Unity Debug.Log later)
        private void Log(string message)
        {
            Console.WriteLine($"[SelectionSystem] {message}");
        }
    }
}
