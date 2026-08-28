## 1. Entry Point and Core Configuration Annotations

- [x] 1.1 Add flow explanation and `// HIGHLIGHT:` anchors in `src/Program.cs` for startup initialization, configuration parsing, and paradigm dispatching
- [x] 1.2 Add explanatory comments and `// HIGHLIGHT:` anchors in `src/LlmConfiguration.cs` and `src/CodingProjectState.cs`

## 2. Loop Paradigm Annotations

- [x] 2.1 Add architectural lifecycle comments and `// HIGHLIGHT:` markers in `src/LoopParadigm/LoopAgentWalkthrough.cs` explaining single-agent setup, prompt structure, tool binding, and autonomous iteration loops
- [x] 2.2 Add flow comments and `// HIGHLIGHT:` anchors in `src/LoopParadigm/LoopDiagnosticWorkspace.cs` detailing test environment preparation and command execution sandboxing

## 3. Graph Paradigm Annotations

- [x] 3.1 Add stage comments and `// HIGHLIGHT:` anchors in `src/GraphParadigm/GraphWorkflowWalkthrough.cs` explaining multi-agent specialization, state accumulation, and human interaction points
- [x] 3.2 Add structural flow comments and `// HIGHLIGHT:` anchors in `src/WorkflowGraph.cs` detailing DAG builder configuration, agent node registration, and conditional transition routing

## 4. Governance, Checkpoint & Tool Annotations

- [x] 4.1 Add flow explanations and `// HIGHLIGHT:` anchors in `src/Governance/MiddlewareGuardrail.cs` detailing function calling interception, schema validation, safety gating, and telemetry
- [x] 4.2 Add flow comments and `// HIGHLIGHT:` anchors in `src/HumanCheckpointStore.cs` explaining state serialization, resumption tokens, and human approval persistence
- [x] 4.3 Add execution flow comments and `// HIGHLIGHT:` anchors in `src/TerminalExecutionTool.cs` detailing process invocation, output streaming, and error handling

## 5. Verification & Testing

- [x] 5.1 Run `dotnet build` to verify clean compilation with zero warnings or errors
- [x] 5.2 Run `dotnet test` to confirm all test suites pass without regression
