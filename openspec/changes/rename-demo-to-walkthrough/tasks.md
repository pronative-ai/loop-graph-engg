## 1. Loop Paradigm Renaming

- [x] 1.1 Rename `src/LoopParadigm/LoopAgentDemo.cs` to `src/LoopParadigm/LoopAgentWalkthrough.cs` and rename class to `LoopAgentWalkthrough`
- [x] 1.2 Update banner string to `"LOOP ENGINEERING WALKTHROUGH"` and comments in `LoopAgentWalkthrough.cs`

## 2. Graph Paradigm Renaming

- [x] 2.1 Rename `src/GraphParadigm/GraphWorkflowDemo.cs` to `src/GraphParadigm/GraphWorkflowWalkthrough.cs` and rename class to `GraphWorkflowWalkthrough`
- [x] 2.2 Update banner string to `"GRAPH ENGINEERING WALKTHROUGH"`, goal to `"Deterministic walkthrough workflow"`, and comments in `GraphWorkflowWalkthrough.cs`

## 3. Governance Middleware Renaming

- [x] 3.1 Update banner string in `src/Governance/MiddlewareGuardrail.cs` to `"GOVERNANCE MIDDLEWARE WALKTHROUGH"` and comments

## 4. Main Entry Point and Shared Code

- [x] 4.1 Update `src/Program.cs` banner to `"=== Microsoft Agent Framework Walkthrough ==="`, menu prompt to `"Select a walkthrough to run:"`, menu items to "Walkthrough", and rename `RunAllDemos()` to `RunAllWalkthroughs()`
- [x] 4.2 Update references in `src/Program.cs` to call `LoopAgentWalkthrough` and `GraphWorkflowWalkthrough`
- [x] 4.3 Update code comments in `WorkflowGraph.cs`, `TerminalExecutionTool.cs`, `ConsoleLogger.cs`, `LlmConfiguration.cs`, and `GlobalUsings.cs` replacing "demo" with "walkthrough"

## 5. Tests and Verification

- [x] 5.1 Update test assertions in `tests/AksAgenticWorkflowConsole.Tests/StateAndToolTests.cs` (change "Build demo" to "Build walkthrough")
- [x] 5.2 Run `dotnet build` and `dotnet test` to verify zero warnings, zero errors, and 100% passing tests
