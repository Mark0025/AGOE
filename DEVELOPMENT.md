# AGOE Development Guide

## Initial Setup

### 1. Install .NET 8 SDK

```bash
brew install --cask dotnet
dotnet --info
```

### 2. Install Unity Hub (for Sprint 1 Unity integration)

Download from: https://unity.com/download

Install Unity LTS (2022.3 or 2023.3 recommended)

## Running Tests (Without Unity)

We've set up plain C# tests that can run before Unity integration:

```bash
# Build the test project
dotnet build AGOE.Tests.csproj

# Run all tests
dotnet run --project AGOE.Tests.csproj

# Watch mode (auto-rebuild on changes)
dotnet watch run --project AGOE.Tests.csproj
```

## Project Structure

```
AGOE/
├── Assets/Code/              # Unity C# code (once Unity is initialized)
│   ├── Commands/            # Command pattern implementation
│   │   ├── ICommand.cs      # ✅ Command interface
│   │   ├── MoveCommand.cs   # ✅ Move command implementation
│   │   └── CommandBus.cs    # ✅ Central command router
│   ├── Core/                # Utilities, event bus
│   ├── Systems/             # Game systems
│   ├── Input/               # Input handling
│   ├── Units/               # Unit logic
│   ├── Map/                 # Grid and terrain
│   ├── UI/                  # UI components
│   └── Tests/
│       ├── EditMode/        # ✅ Unit tests (no Unity runtime)
│       └── PlayMode/        # Integration tests (Unity runtime)
├── AGOE.Tests.csproj        # ✅ Test project
└── TestRunner.cs            # ✅ Simple test runner
```

## Test-Driven Development Workflow

1. **Write test first** (Red)

   ```csharp
   public static void Test_NewFeature()
   {
       // Arrange
       var system = new MySystem();

       // Act
       var result = system.DoSomething();

       // Assert
       Assert(result == expected, "Should return expected value");
   }
   ```

2. **Implement feature** (Green)

3. **Refactor** (Refactor)

4. **Git commit**
   ```bash
   git add .
   git commit -m "feat: implement X with tests"
   git push
   ```

## Sprint 1 Status

### ✅ Completed

- [x] Project structure with .gitignore
- [x] ICommand interface
- [x] MoveCommand implementation
- [x] CommandBus with full routing logic
- [x] Comprehensive tests (8 test cases)
- [x] Test runner setup

### 🚧 Next Steps

1. Install .NET 8 SDK
2. Run tests to verify everything works
3. Initialize Unity project
4. Integrate Unity Test Framework
5. Implement selection system
6. Add NavMesh movement

## Running Tests in Unity (Future)

Once Unity is integrated:

```bash
# From Unity Editor: Window → General → Test Runner
# Or command line:
Unity -runTests -batchmode -projectPath . -testResults results.xml
```

## Git Workflow

**Commit early and often!**

```bash
# After completing a feature with tests
git add .
git commit -m "feat: descriptive message"

# After fixing a bug
git commit -m "fix: describe the fix"

# After adding tests
git commit -m "test: add tests for X"

# Push regularly
git push
```

## Code Quality Standards

- ✅ All public APIs have XML documentation
- ✅ All features have unit tests
- ✅ Tests follow Arrange-Act-Assert pattern
- ✅ Interfaces used for boundaries
- ✅ No Unity dependencies in core logic (yet)

## Current Test Coverage

### CommandBus (8 tests)

- ✅ Subscribe adds handler
- ✅ Route command with subscriber delivers command
- ✅ Route command with multiple subscribers delivers to all
- ✅ Route command with no subscribers doesn't throw
- ✅ Queue command stores command
- ✅ Process queue executes all commands
- ✅ Unsubscribe removes handler
- ✅ Get handler count returns correct count

## Next Session Checklist

- [ ] Run `brew install --cask dotnet`
- [ ] Run `dotnet build AGOE.Tests.csproj`
- [ ] Run `dotnet run --project AGOE.Tests.csproj`
- [ ] Verify all tests pass ✓
- [ ] Initialize Unity project
- [ ] Git commit progress
