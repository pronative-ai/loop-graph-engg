## 1. Global Usings Consolidation

- [x] 1.1 Update `src/GlobalUsings.cs` with all required framework, OpenTelemetry, MAF, and internal namespaces
- [x] 1.2 Create `tests/AksAgenticWorkflowConsole.Tests/GlobalUsings.cs` for test project imports

## 2. Source Files Cleanup

- [x] 2.1 Remove local using statements from all files in `src/` (`Program.cs`, `WorkflowGraph.cs`, `TelemetryConfiguration.cs`, `LoopAgentWalkthrough.cs`, `GraphWorkflowWalkthrough.cs`, `MiddlewareGuardrail.cs`)
- [x] 2.2 Remove local using statements from all test files in `tests/AksAgenticWorkflowConsole.Tests/`

## 3. Build & Test Verification

- [x] 3.1 Run `dotnet build` to verify clean compilation with zero warnings or errors
- [x] 3.2 Run `dotnet test` to verify all 30 unit tests pass
