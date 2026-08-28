## 1. Package Upgrade & MAF Core Integration

- [x] 1.1 Upgrade `Microsoft.Agents.AI` and `Microsoft.Agents.AI.Workflows` package references to `1.13.0` in `src/AksAgenticWorkflowConsole.csproj`
- [x] 1.2 Verify `src/GlobalUsings.cs` imports `Microsoft.Agents.AI` and `Microsoft.Agents.AI.Workflows`

## 2. Official MAF LoopAgent Implementation

- [x] 2.1 Refactor `src/LoopParadigm/LoopAgentWalkthrough.cs` to construct and execute `Microsoft.Agents.AI.LoopAgent` with `CompletionMarkerLoopEvaluator` and `LoopAgentOptions`
- [x] 2.2 Wire streaming iteration output and OpenTelemetry activity tags for `LoopAgent` runs

## 3. Graph Paradigm MAF Workflows

- [x] 3.1 Verify `src/GraphParadigm/GraphWorkflowWalkthrough.cs` and `src/WorkflowGraph.cs` are powered by `Microsoft.Agents.AI.Workflows.WorkflowBuilder` and `Workflow`
- [x] 3.2 Verify `src/Governance/MiddlewareGuardrail.cs` guardrail execution

## 4. Test Suite and Build Verification

- [x] 4.1 Update test suites in `tests/AksAgenticWorkflowConsole.Tests/` to test `Microsoft.Agents.AI.LoopAgent` and `Workflow`
- [x] 4.2 Run `dotnet build` to verify clean compilation with zero warnings or errors
- [x] 4.3 Run `dotnet test` to confirm all unit tests pass
