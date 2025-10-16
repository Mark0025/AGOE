using System;
using System.Collections.Generic;

namespace AGOE.Commands
{
    /// <summary>
    /// Central command routing system.
    /// All user actions flow through this bus to their appropriate handlers.
    /// Benefits:
    /// - Decouples command issuers from command handlers
    /// - Enables testing without Unity MonoBehaviours
    /// - Supports replay/undo functionality
    /// - Allows logging and debugging of all player actions
    /// </summary>
    public class CommandBus
    {
        // Maps command types to their handlers
        private readonly Dictionary<string, List<Action<ICommand>>> _handlers;

        // Queue for commands if we want to process them in batches
        private readonly Queue<ICommand> _commandQueue;

        // Flag to enable/disable logging
        private readonly bool _enableLogging;

        public CommandBus(bool enableLogging = false)
        {
            _handlers = new Dictionary<string, List<Action<ICommand>>>();
            _commandQueue = new Queue<ICommand>();
            _enableLogging = enableLogging;
        }

        /// <summary>
        /// Subscribe a handler to a specific command type.
        /// Multiple handlers can subscribe to the same command type.
        /// </summary>
        /// <param name="commandType">The type of command to handle (e.g., "Move")</param>
        /// <param name="handler">The callback to invoke when this command is received</param>
        public void Subscribe(string commandType, Action<ICommand> handler)
        {
            if (string.IsNullOrEmpty(commandType))
                throw new ArgumentException("Command type cannot be null or empty", nameof(commandType));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!_handlers.ContainsKey(commandType))
            {
                _handlers[commandType] = new List<Action<ICommand>>();
            }

            _handlers[commandType].Add(handler);

            if (_enableLogging)
                Log($"Handler subscribed to command type: {commandType}");
        }

        /// <summary>
        /// Unsubscribe a handler from a command type.
        /// </summary>
        public void Unsubscribe(string commandType, Action<ICommand> handler)
        {
            if (_handlers.ContainsKey(commandType))
            {
                _handlers[commandType].Remove(handler);

                if (_enableLogging)
                    Log($"Handler unsubscribed from command type: {commandType}");
            }
        }

        /// <summary>
        /// Route a command to all subscribed handlers immediately.
        /// </summary>
        public void RouteCommand(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var commandType = command.CommandType;

            if (_enableLogging)
                Log($"Routing command: {commandType} at timestamp {command.Timestamp}");

            if (_handlers.ContainsKey(commandType))
            {
                foreach (var handler in _handlers[commandType])
                {
                    try
                    {
                        handler(command);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error handling command {commandType}: {ex.Message}");
                    }
                }
            }
            else
            {
                if (_enableLogging)
                    Log($"No handlers registered for command type: {commandType}");
            }
        }

        /// <summary>
        /// Queue a command for later processing.
        /// Useful for batching commands or deferring execution.
        /// </summary>
        public void QueueCommand(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            _commandQueue.Enqueue(command);

            if (_enableLogging)
                Log($"Command queued: {command.CommandType}");
        }

        /// <summary>
        /// Process all queued commands.
        /// Call this in your game loop or at specific intervals.
        /// </summary>
        public void ProcessQueue()
        {
            while (_commandQueue.Count > 0)
            {
                var command = _commandQueue.Dequeue();
                RouteCommand(command);
            }
        }

        /// <summary>
        /// Clear all queued commands.
        /// </summary>
        public void ClearQueue()
        {
            _commandQueue.Clear();

            if (_enableLogging)
                Log("Command queue cleared");
        }

        /// <summary>
        /// Get count of handlers for a specific command type.
        /// Useful for testing and debugging.
        /// </summary>
        public int GetHandlerCount(string commandType)
        {
            return _handlers.ContainsKey(commandType) ? _handlers[commandType].Count : 0;
        }

        // Placeholder logging methods (will use Unity Debug.Log later)
        private void Log(string message)
        {
            Console.WriteLine($"[CommandBus] {message}");
        }

        private void LogError(string message)
        {
            Console.WriteLine($"[CommandBus ERROR] {message}");
        }
    }
}
