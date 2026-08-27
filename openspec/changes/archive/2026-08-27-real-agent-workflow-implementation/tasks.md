## 1. LLM Client & Gateway Infrastructure

- [x] 1.1 Update `LlmConfiguration.cs` to normalize endpoint URLs and handle gateway routing correctly
- [x] 1.2 Add connectivity verification test/method to validate `IChatClient` initialization against `.env` gateway settings

## 2. State & Tooling Infrastructure

- [x] 2.1 Enhance `CodingProjectState.cs` to track full workflow state, architecture specifications, backend/frontend code artifacts, and review notes
- [x] 2.2 Refactor `TerminalExecutionTool.cs` to execute compilation diagnostics and return structured compiler output for agent consumption

## 3. Real Loop Agent Autonomous Implementation

- [x] 3.1 Implement live compilation/verification tool binding with `AIFunctionFactory` in `LoopAgentDemo.cs`
- [x] 3.2 Update `LoopAgentDemo.RunAsync` to run real iterative correction with `ChatClientAgent`, displaying live streaming tokens and iteration metrics
- [x] 3.3 Ensure loop demo gracefully handles tool responses, tracks iterations, and displays convergence

## 4. Real DAG Graph Orchestration Implementation

- [x] 4.1 Update `WorkflowGraph.cs` to support typed node execution with asynchronous state passing and parallel execution results aggregation
- [x] 4.2 Define dedicated agent nodes (`ArchitectAgent`, `BackendCoderAgent`, `FrontendCoderAgent`, `ReviewerAgent`) in `GraphWorkflowDemo.cs`
- [x] 4.3 Implement DAG construction in `GraphWorkflowDemo.cs` with parallel split and join synchronization
- [x] 4.4 Connect `GraphWorkflowDemo.RunAsync` to execute the live `AgenticWorkflow<CodingProjectState>` with state flowing through each node

## 5. Real Governance Middleware Guardrail Implementation

- [x] 5.1 Implement `WorkflowMiddleware` pipeline execution in `AgenticWorkflow` that intercepts the `DeploymentNode` transition
- [x] 5.2 Integrate `HumanCheckpointStore` with interactive console approval prompt and ANSI styling in `MiddlewareGuardrail.cs`
- [x] 5.3 Wire the guarded deployment flow so operator approval or rejection correctly controls deployment execution

## 6. Verification & End-to-End Testing

- [x] 6.1 Build the project with `dotnet build` to verify clean compilation with zero warnings and zero errors
- [x] 6.2 Execute and test console demos (Loop Engineering, Graph Engineering, Governance Middleware) to verify live execution
- [x] 6.3 Run `openspec validate` to confirm all planning and spec requirements are satisfied
