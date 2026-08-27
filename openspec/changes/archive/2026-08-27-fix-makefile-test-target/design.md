## Context

Running `make test` previously failed with `MSB1001: Unknown switch --project` and `MSB1003: Specify a project or solution file`. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Fix `Makefile` target `test:` to run `dotnet test tests/AksAgenticWorkflowConsole.Tests/AksAgenticWorkflowConsole.Tests.csproj`.
- Create a root `LoopAgent.sln` referencing both `src/AksAgenticWorkflowConsole.csproj` and `tests/AksAgenticWorkflowConsole.Tests/AksAgenticWorkflowConsole.Tests.csproj`.
- Enable frictionless CLI execution for `make test` and root `dotnet test`.

**Non-Goals:**
- Modifying test assertions or agent implementation code.

## Decisions

### Decision 1: Target the test project in Makefile
- *Design*: Change `Makefile` recipe for `test:` to `dotnet test tests/AksAgenticWorkflowConsole.Tests/AksAgenticWorkflowConsole.Tests.csproj`.
- *Rationale*: Directly specifies the test assembly project without relying on directory discovery or invalid flags.

### Decision 2: Add root solution file `LoopAgent.sln`
- *Design*: Generate `LoopAgent.sln` and attach both `src` and `tests` projects.
- *Rationale*: Enables standard .NET tooling, IDE project hierarchy, and root `dotnet test` / `dotnet build` commands.

## Risks / Trade-offs

- [Risk] Make on Windows requiring GNU make / git bash.
  - *Mitigation*: The recipes use standard cross-platform `dotnet` CLI commands.
