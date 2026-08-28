## Why

Microsoft Agent Framework (MAF) v1.11+ introduces official native agent looping abstractions: `Microsoft.Agents.AI.LoopAgent`, `LoopAgentOptions`, and `LoopEvaluator` implementations (`CompletionMarkerLoopEvaluator`, `DelegateLoopEvaluator`, `AIJudgeLoopEvaluator`) as documented in [Microsoft Learn: Agent Looping](https://learn.microsoft.com/en-us/agent-framework/agents/looping?pivots=programming-language-csharp). The project should adopt this official `LoopAgent` abstraction alongside MAF's built-in `Microsoft.Agents.AI.Workflows.WorkflowBuilder` and `Workflow`.

## What Changes

- **Upgrade MAF Package References**: Update `Microsoft.Agents.AI` and `Microsoft.Agents.AI.Workflows` to `1.13.0` to access the official `Microsoft.Agents.AI.LoopAgent` and loop evaluator types.
- **Implement Official MAF `LoopAgent`**: Refactor `LoopAgentWalkthrough` to wrap the base `ChatClientAgent` with `new LoopAgent(baseAgent, evaluator, new LoopAgentOptions { MaxIterations = 5 })` using official MAF loop evaluators (`CompletionMarkerLoopEvaluator` / `DelegateLoopEvaluator`).
- **Graph Paradigm with MAF `WorkflowBuilder`**: Continue using official `Microsoft.Agents.AI.Workflows.WorkflowBuilder` and `Workflow` for the DAG orchestration.
- **Governance Middleware & Checks**: Align guardrails and event streams across both MAF `LoopAgent` and `Workflow` paradigms.
- **Update Unit Tests**: Add unit tests verifying `Microsoft.Agents.AI.LoopAgent` and `Workflow` executions.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agent-framework-demo`: Explicitly mandate the use of Microsoft Agent Framework's official `Microsoft.Agents.AI.LoopAgent` and built-in `Workflow` engine.
- `agentic-workflow-core`: Maintain MAF native `WorkflowBuilder` and `Workflow` requirements for graph orchestration.

## Impact

- **Affected Source Files**:
  - `src/AksAgenticWorkflowConsole.csproj` (Upgrade `Microsoft.Agents.AI` and `Microsoft.Agents.AI.Workflows` to `1.13.0`)
  - `src/LoopParadigm/LoopAgentWalkthrough.cs` (Replaced with official `Microsoft.Agents.AI.LoopAgent`)
  - `src/WorkflowGraph.cs` (MAF `WorkflowBuilder` and `Workflow`)
  - `tests/AksAgenticWorkflowConsole.Tests/LoopDiagnosticTests.cs` and `WorkflowGraphTests.cs`
- **Dependencies**: References official `Microsoft.Agents.AI` (1.13.0) and `Microsoft.Agents.AI.Workflows` (1.13.0).
