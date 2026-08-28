## Context

Several `.cs` files across the codebase retain local file-level `using` directives despite the presence of `GlobalUsings.cs`. Consolidating all namespaces into project-level `GlobalUsings.cs` simplifies file structure and enforces a single source of truth for namespace imports.

## Goals / Non-Goals

**Goals:**
- Expand `src/GlobalUsings.cs` to include all namespaces required by any file in the main application.
- Add `tests/AksAgenticWorkflowConsole.Tests/GlobalUsings.cs` to manage all test-project namespace imports.
- Strip all file-level `using` statements from every `.cs` file in `src/` and `tests/`.
- Ensure `dotnet build` and `dotnet test` compile and pass cleanly with 0 warnings/errors.

**Non-Goals:**
- Modifying business logic, agent workflows, or test logic.

## Decisions

### Decision 1: Comprehensive `src/GlobalUsings.cs`
- **Choice**: Declare all framework, OpenTelemetry, MAF, and internal sub-namespaces globally.
- **Rationale**: Completely frees individual files from needing any header imports.

### Decision 2: Dedicated `tests/AksAgenticWorkflowConsole.Tests/GlobalUsings.cs`
- **Choice**: Introduce a test-scoped `GlobalUsings.cs` for `Xunit` and test fixtures.
- **Rationale**: Isolates test dependencies while keeping test files clean.

## Risks / Trade-offs

- **[Risk]** Namespace shadowing or collisions.
  - **Mitigation**: Verified unique namespace identifiers across projects; verified build compilation.
