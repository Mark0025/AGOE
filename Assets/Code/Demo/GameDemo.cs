using System;
using System.Collections.Generic;
using AGOE.Commands;
using AGOE.Systems;
using AGOE.Units;

namespace AGOE.Demo
{
    /// <summary>
    /// Interactive console demo showing the game systems in action.
    /// Run this to see SelectionSystem and CommandBus working together.
    /// </summary>
    public class GameDemo
    {
        private readonly SelectionSystem _selectionSystem;
        private readonly CommandBus _commandBus;
        private readonly List<Unit> _units;
        private float _gameTime;

        public GameDemo()
        {
            _selectionSystem = new SelectionSystem(enableLogging: true);
            _commandBus = new CommandBus(enableLogging: true);
            _units = new List<Unit>();
            _gameTime = 0f;

            // Subscribe to selection changes
            _selectionSystem.OnSelectionChanged += OnSelectionChanged;

            // Subscribe to commands
            _commandBus.Subscribe("Select", HandleSelectCommand);
            _commandBus.Subscribe("Move", HandleMoveCommand);
        }

        public void Run()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════╗");
            Console.WriteLine("║     AGOE - Game Systems Demo              ║");
            Console.WriteLine("║     (Console Simulation - No Graphics)    ║");
            Console.WriteLine("╚════════════════════════════════════════════╝\n");

            // Spawn some units
            SpawnUnit("Villager", 0);
            SpawnUnit("Villager", 0);
            SpawnUnit("Soldier", 0);
            SpawnUnit("Archer", 0);

            Console.WriteLine($"\n✓ Spawned {_units.Count} units for Player 0\n");
            PrintUnits();

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("DEMO: Selection System");
            Console.WriteLine(new string('─', 50) + "\n");

            // Demo 1: Single selection
            Console.WriteLine("→ Selecting single unit (ID: 1)...");
            var selectCmd = new SelectCommand(1, SelectionMode.Replace, _gameTime);
            _commandBus.RouteCommand(selectCmd);

            Console.WriteLine("\n→ Adding another unit to selection (ID: 2)...");
            selectCmd = new SelectCommand(2, SelectionMode.Add, _gameTime);
            _commandBus.RouteCommand(selectCmd);

            Console.WriteLine("\n→ Selecting multiple units via area select...");
            selectCmd = new SelectCommand(new List<int> { 3, 4 }, SelectionMode.Replace, _gameTime);
            _commandBus.RouteCommand(selectCmd);

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("DEMO: Movement Commands");
            Console.WriteLine(new string('─', 50) + "\n");

            // Demo 2: Issue move command
            Console.WriteLine("→ Issuing move command to selected units...");
            var targetPos = new Vector3Placeholder(100, 0, 50);
            var selectedIds = GetSelectedUnitIds();
            var moveCmd = new MoveCommand(targetPos, selectedIds, _gameTime);
            _commandBus.RouteCommand(moveCmd);

            Console.WriteLine("\n→ Simulating unit movement...");
            SimulateMovement();

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("DEMO: Combat System");
            Console.WriteLine(new string('─', 50) + "\n");

            // Demo 3: Damage and death
            Console.WriteLine("→ Unit 1 takes 60 damage...");
            _units[0].TakeDamage(60);
            Console.WriteLine($"   {_units[0]}");

            Console.WriteLine("\n→ Unit 1 takes 50 more damage (dies)...");
            var died = _units[0].TakeDamage(50);
            Console.WriteLine($"   {_units[0]}");
            if (died)
            {
                Console.WriteLine("   ⚰️  Unit 1 has died!");
            }

            Console.WriteLine("\n→ Trying to select dead unit...");
            selectCmd = new SelectCommand(1, SelectionMode.Replace, _gameTime);
            _commandBus.RouteCommand(selectCmd);

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("DEMO: Current Game State");
            Console.WriteLine(new string('─', 50) + "\n");

            PrintUnits();

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("✓ Demo Complete!");
            Console.WriteLine(new string('─', 50));
            Console.WriteLine("\nThis demonstrates:");
            Console.WriteLine("  • Command Bus routing commands to systems");
            Console.WriteLine("  • Selection System managing unit selection");
            Console.WriteLine("  • Unit state management (Idle → Moving)");
            Console.WriteLine("  • Combat system (damage, death)");
            Console.WriteLine("  • Dead units cannot be selected");
            Console.WriteLine("\nNext: Integrate with Unity for visuals!\n");
        }

        private void SpawnUnit(string unitType, int playerId)
        {
            var id = _units.Count + 1;
            var unit = new Unit(id, unitType, playerId);
            // Set random-ish positions
            unit.Position = new Vector3Placeholder(id * 10, 0, id * 5);
            _units.Add(unit);
        }

        private void PrintUnits()
        {
            Console.WriteLine("Current Units:");
            foreach (var unit in _units)
            {
                var selectedMark = unit.IsSelected ? "[SELECTED]" : "";
                var stateMark = unit.State == UnitState.Dead ? "💀" :
                               unit.State == UnitState.Moving ? "🏃" : "🧍";
                Console.WriteLine($"  {stateMark} {unit} {selectedMark}");
            }
        }

        private List<int> GetSelectedUnitIds()
        {
            var ids = new List<int>();
            foreach (var unit in _selectionSystem.GetSelectedUnits())
            {
                ids.Add(unit.Id);
            }
            return ids;
        }

        private void OnSelectionChanged(IReadOnlyCollection<ISelectable> selected)
        {
            Console.WriteLine($"   [EVENT] Selection changed - {selected.Count} unit(s) selected");
        }

        private void HandleSelectCommand(ICommand command)
        {
            var selectCmd = command as SelectCommand;
            if (selectCmd == null) return;

            switch (selectCmd.Mode)
            {
                case SelectionMode.Replace:
                    _selectionSystem.ClearSelection();
                    foreach (var id in selectCmd.EntityIds)
                    {
                        var unit = _units.Find(u => u.Id == id);
                        if (unit != null)
                            _selectionSystem.AddToSelection(unit);
                    }
                    break;

                case SelectionMode.Add:
                    foreach (var id in selectCmd.EntityIds)
                    {
                        var unit = _units.Find(u => u.Id == id);
                        if (unit != null)
                            _selectionSystem.AddToSelection(unit);
                    }
                    break;

                case SelectionMode.Toggle:
                    foreach (var id in selectCmd.EntityIds)
                    {
                        var unit = _units.Find(u => u.Id == id);
                        if (unit != null)
                        {
                            if (_selectionSystem.IsSelected(unit))
                                _selectionSystem.RemoveFromSelection(unit);
                            else
                                _selectionSystem.AddToSelection(unit);
                        }
                    }
                    break;
            }
        }

        private void HandleMoveCommand(ICommand command)
        {
            var moveCmd = command as MoveCommand;
            if (moveCmd == null) return;

            Console.WriteLine($"   Moving {moveCmd.UnitIds.Count} unit(s) to {moveCmd.TargetPosition}");

            // Update unit states
            foreach (var id in moveCmd.UnitIds)
            {
                var unit = _units.Find(u => u.Id == id);
                if (unit != null && unit.State != UnitState.Dead)
                {
                    unit.State = UnitState.Moving;
                    Console.WriteLine($"   → Unit {id} state: Idle → Moving");
                }
            }
        }

        private void SimulateMovement()
        {
            foreach (var unit in _units)
            {
                if (unit.State == UnitState.Moving)
                {
                    // Simulate arrival at destination
                    unit.State = UnitState.Idle;
                    Console.WriteLine($"   → Unit {unit.Id} arrived at destination (Moving → Idle)");
                }
            }
        }
    }
}
