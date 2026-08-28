## Why

Individual `.cs` files across `src/` and `tests/` currently contain redundant local `using` statements at the top of each file. The project's code organization standard mandates that all using directives must be consolidated into project-level `GlobalUsings.cs` files, keeping individual source files clean, focused, and free from repetitive header imports.

## What Changes

- **Consolidate Global Usings in `src/GlobalUsings.cs`**: Add all necessary global using directives (`OpenTelemetry.*`, `AgenticWorkflowConsole.*`, `System.Diagnostics`, `System.Text`, `Microsoft.Agents.AI.Workflows.InProc`, etc.) so that no `src/` file requires local `using` statements.
- **Create Global Usings for Test Project `tests/.../GlobalUsings.cs`**: Create a test-wide `GlobalUsings.cs` containing `Xunit`, `AgenticWorkflowConsole.*`, etc.
- **Clean Up Individual Source Files**: Remove all file-scoped `using` directives from every `.cs` file in `src/` and `tests/`.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `code-organization`: Enforce zero local using directives in all source files with all namespaces managed in `GlobalUsings.cs`.

## Impact

- **Affected Source Files**:
  - `src/GlobalUsings.cs`
  - `src/Program.cs`
  - `src/WorkflowGraph.cs`
  - `src/Shared/TelemetryConfiguration.cs`
  - `src/LoopParadigm/LoopAgentWalkthrough.cs`
  - `src/GraphParadigm/GraphWorkflowWalkthrough.cs`
  - `src/Governance/MiddlewareGuardrail.cs`
  - `tests/AksAgenticWorkflowConsole.Tests/GlobalUsings.cs` (New)
  - `tests/AksAgenticWorkflowConsole.Tests/*.cs` (Cleaned)
- **Dependencies**: No dependency changes.
