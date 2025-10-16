using System;
using System.Linq;
using AGOE.Systems;
using AGOE.Units;

namespace AGOE.Tests
{
    /// <summary>
    /// Unit tests for SelectionSystem.
    /// Tests selection, multi-selection, clearing, and event firing.
    /// </summary>
    public class SelectionSystemTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("Running SelectionSystem Tests...\n");

            Test_SelectSingle_SelectsEntity();
            Test_SelectSingle_ClearsPreviousSelection();
            Test_AddToSelection_AddsWithoutClearing();
            Test_AddMultipleToSelection_SelectsMultipleUnits();
            Test_RemoveFromSelection_RemovesEntity();
            Test_ClearSelection_ClearsAllSelected();
            Test_IsSelected_ReturnsCorrectValue();
            Test_GetSelectedUnits_ReturnsOnlyUnits();
            Test_SelectionCount_ReturnsCorrectCount();
            Test_OnSelectionChanged_FiresWhenSelectionChanges();
            Test_CannotSelectDeadUnit();
            Test_SelectAll_SelectsAllEntities();

            Console.WriteLine("\n✓ All SelectionSystem tests passed!");
        }

        public static void Test_SelectSingle_SelectsEntity()
        {
            // Arrange
            var system = new SelectionSystem();
            var unit = new Unit(1, "Villager", 0);

            // Act
            system.SelectSingle(unit);

            // Assert
            Assert(unit.IsSelected, "Unit should be selected");
            Assert(system.SelectionCount == 1, "Selection count should be 1");
            Assert(system.HasSelection, "Should have selection");
            Assert(system.IsSelected(unit), "Unit should be in selection");

            Console.WriteLine("✓ Test_SelectSingle_SelectsEntity passed");
        }

        public static void Test_SelectSingle_ClearsPreviousSelection()
        {
            // Arrange
            var system = new SelectionSystem();
            var unit1 = new Unit(1, "Villager", 0);
            var unit2 = new Unit(2, "Soldier", 0);

            // Act
            system.SelectSingle(unit1);
            system.SelectSingle(unit2);

            // Assert
            Assert(!unit1.IsSelected, "First unit should not be selected");
            Assert(unit2.IsSelected, "Second unit should be selected");
            Assert(system.SelectionCount == 1, "Selection count should be 1");

            Console.WriteLine("✓ Test_SelectSingle_ClearsPreviousSelection passed");
        }

        public static void Test_AddToSelection_AddsWithoutClearing()
        {
            // Arrange
            var system = new SelectionSystem();
            var unit1 = new Unit(1, "Villager", 0);
            var unit2 = new Unit(2, "Soldier", 0);

            // Act
            system.SelectSingle(unit1);
            system.AddToSelection(unit2);

            // Assert
            Assert(unit1.IsSelected, "First unit should still be selected");
            Assert(unit2.IsSelected, "Second unit should be selected");
            Assert(system.SelectionCount == 2, "Selection count should be 2");

            Console.WriteLine("✓ Test_AddToSelection_AddsWithoutClearing passed");
        }

        public static void Test_AddMultipleToSelection_SelectsMultipleUnits()
        {
            // Arrange
            var system = new SelectionSystem();
            var units = new[]
            {
                new Unit(1, "Villager", 0),
                new Unit(2, "Villager", 0),
                new Unit(3, "Soldier", 0)
            };

            // Act
            system.AddMultipleToSelection(units);

            // Assert
            Assert(system.SelectionCount == 3, "Should have 3 units selected");
            foreach (var unit in units)
            {
                Assert(unit.IsSelected, $"Unit {unit.Id} should be selected");
            }

            Console.WriteLine("✓ Test_AddMultipleToSelection_SelectsMultipleUnits passed");
        }

        public static void Test_RemoveFromSelection_RemovesEntity()
        {
            // Arrange
            var system = new SelectionSystem();
            var unit1 = new Unit(1, "Villager", 0);
            var unit2 = new Unit(2, "Soldier", 0);

            system.AddToSelection(unit1);
            system.AddToSelection(unit2);

            // Act
            system.RemoveFromSelection(unit1);

            // Assert
            Assert(!unit1.IsSelected, "First unit should not be selected");
            Assert(unit2.IsSelected, "Second unit should still be selected");
            Assert(system.SelectionCount == 1, "Selection count should be 1");

            Console.WriteLine("✓ Test_RemoveFromSelection_RemovesEntity passed");
        }

        public static void Test_ClearSelection_ClearsAllSelected()
        {
            // Arrange
            var system = new SelectionSystem();
            var units = new[]
            {
                new Unit(1, "Villager", 0),
                new Unit(2, "Villager", 0),
                new Unit(3, "Soldier", 0)
            };

            system.AddMultipleToSelection(units);

            // Act
            system.ClearSelection();

            // Assert
            Assert(system.SelectionCount == 0, "Selection count should be 0");
            Assert(!system.HasSelection, "Should not have selection");
            foreach (var unit in units)
            {
                Assert(!unit.IsSelected, $"Unit {unit.Id} should not be selected");
            }

            Console.WriteLine("✓ Test_ClearSelection_ClearsAllSelected passed");
        }

        public static void Test_IsSelected_ReturnsCorrectValue()
        {
            // Arrange
            var system = new SelectionSystem();
            var unit1 = new Unit(1, "Villager", 0);
            var unit2 = new Unit(2, "Soldier", 0);

            system.SelectSingle(unit1);

            // Assert
            Assert(system.IsSelected(unit1), "Unit1 should be selected");
            Assert(!system.IsSelected(unit2), "Unit2 should not be selected");
            Assert(!system.IsSelected(null), "Null should return false");

            Console.WriteLine("✓ Test_IsSelected_ReturnsCorrectValue passed");
        }

        public static void Test_GetSelectedUnits_ReturnsOnlyUnits()
        {
            // Arrange
            var system = new SelectionSystem();
            var units = new[]
            {
                new Unit(1, "Villager", 0),
                new Unit(2, "Villager", 0),
                new Unit(3, "Soldier", 0)
            };

            system.AddMultipleToSelection(units);

            // Act
            var selectedUnits = system.GetSelectedUnits();

            // Assert
            Assert(selectedUnits.Count == 3, "Should have 3 selected units");
            Assert(selectedUnits.All(u => u is Unit), "All should be Unit type");

            Console.WriteLine("✓ Test_GetSelectedUnits_ReturnsOnlyUnits passed");
        }

        public static void Test_SelectionCount_ReturnsCorrectCount()
        {
            // Arrange
            var system = new SelectionSystem();
            var unit1 = new Unit(1, "Villager", 0);
            var unit2 = new Unit(2, "Soldier", 0);

            // Assert initial state
            Assert(system.SelectionCount == 0, "Initial count should be 0");

            // Act & Assert - add one
            system.AddToSelection(unit1);
            Assert(system.SelectionCount == 1, "Count should be 1");

            // Act & Assert - add another
            system.AddToSelection(unit2);
            Assert(system.SelectionCount == 2, "Count should be 2");

            // Act & Assert - remove one
            system.RemoveFromSelection(unit1);
            Assert(system.SelectionCount == 1, "Count should be 1");

            // Act & Assert - clear
            system.ClearSelection();
            Assert(system.SelectionCount == 0, "Count should be 0");

            Console.WriteLine("✓ Test_SelectionCount_ReturnsCorrectCount passed");
        }

        public static void Test_OnSelectionChanged_FiresWhenSelectionChanges()
        {
            // Arrange
            var system = new SelectionSystem();
            var unit = new Unit(1, "Villager", 0);
            var eventFiredCount = 0;

            system.OnSelectionChanged += (entities) => eventFiredCount++;

            // Act - various selection changes
            system.SelectSingle(unit);
            system.ClearSelection();
            system.AddToSelection(unit);
            system.RemoveFromSelection(unit);

            // Assert
            Assert(eventFiredCount == 4, "Event should have fired 4 times");

            Console.WriteLine("✓ Test_OnSelectionChanged_FiresWhenSelectionChanges passed");
        }

        public static void Test_CannotSelectDeadUnit()
        {
            // Arrange
            var system = new SelectionSystem();
            var unit = new Unit(1, "Villager", 0);

            // Kill the unit
            unit.TakeDamage(100);

            // Act
            system.SelectSingle(unit);

            // Assert
            Assert(!unit.IsSelected, "Dead unit should not be selected");
            Assert(system.SelectionCount == 0, "Selection count should be 0");

            Console.WriteLine("✓ Test_CannotSelectDeadUnit passed");
        }

        public static void Test_SelectAll_SelectsAllEntities()
        {
            // Arrange
            var system = new SelectionSystem();
            var units = new[]
            {
                new Unit(1, "Villager", 0),
                new Unit(2, "Villager", 0),
                new Unit(3, "Soldier", 0),
                new Unit(4, "Soldier", 0)
            };

            // Act
            system.SelectAll(units);

            // Assert
            Assert(system.SelectionCount == 4, "Should have 4 units selected");
            foreach (var unit in units)
            {
                Assert(unit.IsSelected, $"Unit {unit.Id} should be selected");
            }

            Console.WriteLine("✓ Test_SelectAll_SelectsAllEntities passed");
        }

        // Simple assertion helper
        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception($"Assertion failed: {message}");
            }
        }
    }
}
