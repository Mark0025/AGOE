using System;
using System.Collections.Generic;
using AGOE.Commands;

namespace AGOE.Tests
{
    /// <summary>
    /// Unit tests for CommandBus.
    /// These tests verify the core command routing functionality without Unity dependencies.
    ///
    /// Note: Once Unity Test Framework is integrated, add [TestFixture] attribute
    /// and replace SimpleTest methods with [Test] attributes.
    /// </summary>
    public class CommandBusTests
    {
        /// <summary>
        /// Run all tests. In Unity, each method will be a separate [Test].
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("Running CommandBus Tests...\n");

            Test_Subscribe_AddsHandler();
            Test_RouteCommand_WithSubscriber_DeliversCommand();
            Test_RouteCommand_WithMultipleSubscribers_DeliversToAll();
            Test_RouteCommand_WithNoSubscribers_DoesNotThrow();
            Test_QueueCommand_StoresCommand();
            Test_ProcessQueue_ExecutesAllCommands();
            Test_Unsubscribe_RemovesHandler();
            Test_GetHandlerCount_ReturnsCorrectCount();

            Console.WriteLine("\n✓ All CommandBus tests passed!");
        }

        public static void Test_Subscribe_AddsHandler()
        {
            // Arrange
            var bus = new CommandBus();
            var handlerCalled = false;

            // Act
            bus.Subscribe("TestCommand", cmd => handlerCalled = true);

            // Assert
            Assert(bus.GetHandlerCount("TestCommand") == 1, "Handler count should be 1");
            Console.WriteLine("✓ Test_Subscribe_AddsHandler passed");
        }

        public static void Test_RouteCommand_WithSubscriber_DeliversCommand()
        {
            // Arrange
            var bus = new CommandBus();
            ICommand receivedCommand = null;
            bus.Subscribe("Move", cmd => receivedCommand = cmd);

            var targetPos = new Vector3Placeholder(10, 0, 5);
            var unitIds = new List<int> { 1, 2, 3 };
            var command = new MoveCommand(targetPos, unitIds, 1.5f);

            // Act
            bus.RouteCommand(command);

            // Assert
            Assert(receivedCommand != null, "Command should be received");
            Assert(receivedCommand is MoveCommand, "Received command should be MoveCommand");

            var moveCmd = receivedCommand as MoveCommand;
            Assert(moveCmd.UnitIds.Count == 3, "Should have 3 unit IDs");
            Assert(moveCmd.TargetPosition.X == 10, "Target X should be 10");

            Console.WriteLine("✓ Test_RouteCommand_WithSubscriber_DeliversCommand passed");
        }

        public static void Test_RouteCommand_WithMultipleSubscribers_DeliversToAll()
        {
            // Arrange
            var bus = new CommandBus();
            var handler1Called = false;
            var handler2Called = false;

            bus.Subscribe("Move", cmd => handler1Called = true);
            bus.Subscribe("Move", cmd => handler2Called = true);

            var command = new MoveCommand(
                new Vector3Placeholder(0, 0, 0),
                new List<int>(),
                0
            );

            // Act
            bus.RouteCommand(command);

            // Assert
            Assert(handler1Called, "Handler 1 should be called");
            Assert(handler2Called, "Handler 2 should be called");
            Assert(bus.GetHandlerCount("Move") == 2, "Should have 2 handlers");

            Console.WriteLine("✓ Test_RouteCommand_WithMultipleSubscribers_DeliversToAll passed");
        }

        public static void Test_RouteCommand_WithNoSubscribers_DoesNotThrow()
        {
            // Arrange
            var bus = new CommandBus();
            var command = new MoveCommand(
                new Vector3Placeholder(0, 0, 0),
                new List<int>(),
                0
            );

            // Act & Assert - should not throw
            try
            {
                bus.RouteCommand(command);
                Console.WriteLine("✓ Test_RouteCommand_WithNoSubscribers_DoesNotThrow passed");
            }
            catch (Exception ex)
            {
                Assert(false, $"Should not throw exception: {ex.Message}");
            }
        }

        public static void Test_QueueCommand_StoresCommand()
        {
            // Arrange
            var bus = new CommandBus();
            var handlerCalled = false;
            bus.Subscribe("Move", cmd => handlerCalled = true);

            var command = new MoveCommand(
                new Vector3Placeholder(0, 0, 0),
                new List<int>(),
                0
            );

            // Act
            bus.QueueCommand(command);

            // Assert - command queued but not processed yet
            Assert(!handlerCalled, "Handler should not be called yet");
            Console.WriteLine("✓ Test_QueueCommand_StoresCommand passed");
        }

        public static void Test_ProcessQueue_ExecutesAllCommands()
        {
            // Arrange
            var bus = new CommandBus();
            var commandsReceived = 0;
            bus.Subscribe("Move", cmd => commandsReceived++);

            // Queue multiple commands
            for (int i = 0; i < 5; i++)
            {
                bus.QueueCommand(new MoveCommand(
                    new Vector3Placeholder(i, 0, 0),
                    new List<int>(),
                    i
                ));
            }

            // Act
            bus.ProcessQueue();

            // Assert
            Assert(commandsReceived == 5, "Should receive 5 commands");
            Console.WriteLine("✓ Test_ProcessQueue_ExecutesAllCommands passed");
        }

        public static void Test_Unsubscribe_RemovesHandler()
        {
            // Arrange
            var bus = new CommandBus();
            Action<ICommand> handler = cmd => { };

            bus.Subscribe("Move", handler);
            Assert(bus.GetHandlerCount("Move") == 1, "Should have 1 handler");

            // Act
            bus.Unsubscribe("Move", handler);

            // Assert
            Assert(bus.GetHandlerCount("Move") == 0, "Should have 0 handlers");
            Console.WriteLine("✓ Test_Unsubscribe_RemovesHandler passed");
        }

        public static void Test_GetHandlerCount_ReturnsCorrectCount()
        {
            // Arrange
            var bus = new CommandBus();

            // Act & Assert
            Assert(bus.GetHandlerCount("NonExistent") == 0, "Non-existent type should have 0 handlers");

            bus.Subscribe("Move", cmd => { });
            Assert(bus.GetHandlerCount("Move") == 1, "Move should have 1 handler");

            bus.Subscribe("Move", cmd => { });
            Assert(bus.GetHandlerCount("Move") == 2, "Move should have 2 handlers");

            Console.WriteLine("✓ Test_GetHandlerCount_ReturnsCorrectCount passed");
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
