# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**AGOE** (Age of Empires) is a learning project to master C# development by building a Real-Time Strategy (RTS) game from scratch using Unity. This is a ground-up educational implementation, not a clone.

**Current Status**: Planning phase. The codebase contains planning documentation but no implementation code yet.

## Technology Stack

- **Engine**: Unity LTS (targeting 2022 or 2023)
- **Language**: C# 10/11 with .NET 8
- **Key Patterns**: SOLID principles, event-driven architecture, command bus pattern
- **Testing**: Unity Test Framework (EditMode & PlayMode) + NSubstitute for mocking
- **Data Architecture**: ScriptableObjects for game data, JSON for save/load
- **AI/Pathfinding**: Unity NavMesh (initial), custom A* for grid-based pathfinding (later)

## Development Commands

### Local C# Development (without Unity)
```bash
# Create new console app
dotnet new console -n <AppName>

# Run application
dotnet run

# Run with hot reload (watch mode)
dotnet watch

# Run tests
dotnet test

# Restore dependencies
dotnet restore

# Build release
dotnet publish -c Release
```

### Docker Workflow
```bash
# Build container (Apple Silicon native)
docker build -t <app-name> .

# Run container
docker run --rm <app-name>

# For web APIs
docker run --rm -p 8080:8080 <api-name>
```

### Unity Development (once initialized)
```bash
# Run tests (headless)
Unity -runTests -batchmode -projectPath . -testResults results.xml

# Build project (headless)
Unity -quit -batchmode -projectPath . -buildTarget StandaloneOSX -executeMethod BuildScript.Build
```

## Planned Architecture

### Core Design Patterns

**Command Bus Architecture**
- All user actions route through a centralized command bus
- Commands implement `ICommand` interface
- Systems subscribe to specific command types
- Enables decoupling, testing, and replay functionality

**Event-Driven Systems**
- Core game loop uses Unity's MonoBehaviour lifecycle
- Custom event bus for cross-system communication
- Action<T> delegates for loose coupling

**Dependency Injection at Boundaries**
- Systems communicate via interfaces (ISelectable, IMovable)
- MonoBehaviours hold references to plain C# systems
- ScriptableObjects inject configuration data

### Planned Project Structure
```
Assets/
├── Code/
│   ├── Core/          # EventBus, utilities, logging wrappers
│   ├── Input/         # Input mapping, selection system
│   ├── Map/           # Grid system, tiles, fog of war
│   ├── Units/         # Unit model, state machine, movement, health
│   ├── Commands/      # ICommand, MoveCommand, AttackCommand, CommandBus
│   ├── Systems/       # SelectionSystem, CommandSystem, FogOfWarSystem
│   └── UI/            # HUD, resource panels, minimap
├── Art/               # Sprites, models, textures
├── Prefabs/           # Unit prefabs, building prefabs
├── Scenes/            # Game scenes
└── ScriptableObjects/ # Unit data, building data, game config
```

## Development Milestones

### Sprint 1-2: Core Loop (First Priority)
- Camera control (pan/zoom)
- Marquee selection system
- Right-click movement via NavMesh
- Command bus implementation
- Unit state management
- **Tests**: Selection logic, command dispatch, movement state

### Sprint 3: Economy
- Resource gathering (wood, food)
- Drop-off buildings
- Resource UI panel

### Sprint 4: Production
- Building placement with grid validation
- Production queue system
- Rally points

### Sprint 5: Combat
- Attack system with targeting
- Health and damage
- Basic combat AI (aggro, chase)

### Sprint 6: Fog of War
- Vision system per unit
- Fog rendering
- Minimap integration

### Sprint 7: Skirmish
- Win/lose conditions
- Enemy AI
- Save/load system

## C# Design Principles for This Project

**Type Safety**
- Use interfaces for boundaries (ICommand, ISelectable)
- Leverage compiler for type checking (no runtime type discovery)
- Prefer explicit types over `var` for public APIs

**Unity-Specific Patterns**
- ScriptableObjects for data-driven design
- MonoBehaviour for Unity lifecycle integration only
- Plain C# classes for game logic (more testable)

**Event Handling**
- Use `Action<T>` delegates for pub/sub
- Command pattern for all user actions
- Event bus for cross-cutting concerns

**Async Operations**
- Use `async/await` for I/O (save/load)
- Coroutines for Unity-specific timing
- Avoid `async void` except for event handlers

## Testing Strategy

**Unit Tests (EditMode)**
- Test game logic in plain C# classes
- Mock Unity APIs using interfaces
- Fast feedback loop without entering Play mode

**Integration Tests (PlayMode)**
- Test MonoBehaviour interactions
- Validate Unity physics/NavMesh integration
- Scene-based tests for full game loop

**Example Test Structure**
```csharp
[TestFixture]
public class CommandBusTests
{
    [Test]
    public void RouteCommand_WithSubscriber_DeliversCommand()
    {
        // Arrange
        var bus = new CommandBus();
        var command = new MoveCommand(targetPos);

        // Act
        bus.RouteCommand(command);

        // Assert
        Assert.IsTrue(commandReceived);
    }
}
```

## Learning Focus

This project is designed to teach:
- **C# fundamentals** through practical game development
- **SOLID principles** in a real-world context
- **Unity architecture** beyond basic tutorials
- **Test-driven development** in game engines
- **Pattern application** (Command, Observer, State)

When implementing features:
1. Start with interfaces and tests
2. Implement logic in plain C# first
3. Integrate with Unity MonoBehaviours last
4. Refactor for SOLID compliance
5. Add integration tests for Unity-specific behavior

## Key Documentation

- Planning document: `DEV_MAN/Plann.v.0.0.1.md` (3,377 lines of detailed sprint planning and C# learning content)
- C# vs Python comparisons and mental model shifts included in planning doc
- Architecture decisions and rationale documented inline

## Notes for AI Assistants

- This is a **learning project** - prioritize teaching moments and explanations
- Follow SOLID principles strictly - this is practice for professional development
- Write tests first when implementing new systems
- Explain Unity-specific concepts when they appear
- Reference the planning document for architectural decisions
- Keep MonoBehaviours thin - push logic into testable C# classes
