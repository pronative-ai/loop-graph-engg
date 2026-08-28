## Context

Microsoft Learn provides official guidance for agent looping: [Agent Looping in Microsoft Agent Framework](https://learn.microsoft.com/en-us/agent-framework/agents/looping?pivots=programming-language-csharp). MAF includes `Microsoft.Agents.AI.LoopAgent`, `LoopAgentOptions`, and built-in evaluators (`CompletionMarkerLoopEvaluator`, `DelegateLoopEvaluator`, `AIJudgeLoopEvaluator`).

## Goals / Non-Goals

**Goals:**
- Update `src/AksAgenticWorkflowConsole.csproj` to reference `Microsoft.Agents.AI` (1.13.0) and `Microsoft.Agents.AI.Workflows` (1.13.0).
- Use MAF's official `Microsoft.Agents.AI.LoopAgent` in `src/LoopParadigm/LoopAgentWalkthrough.cs`, wrapping the developer `ChatClientAgent` with `new LoopAgent(baseAgent, evaluator, new LoopAgentOptions { MaxIterations = 5 })`.
- Use `CompletionMarkerLoopEvaluator("STATUS: [PASS - VERIFIED]")` or `DelegateLoopEvaluator` for completion conditions.
- Continue using `Microsoft.Agents.AI.Workflows.WorkflowBuilder` and `Workflow` for the Graph paradigm.
- Maintain OpenTelemetry spans, streaming logging, and unit tests.

**Non-Goals:**
- Removing live LLM evaluation or interactive console features.

## Decisions

### Decision 1: Upgrade to MAF 1.13.0
- **Choice**: Upgrade `Microsoft.Agents.AI` and `Microsoft.Agents.AI.Workflows` to version 1.13.0.
- **Rationale**: `1.13.0` contains the production `LoopAgent` and `LoopEvaluator` classes documented on Microsoft Learn.

### Decision 2: Official `LoopAgent` Composition
- **Choice**:
  ```csharp
  AIAgent baseAgent = new ChatClientAgent(chatClient, instructions, name: "LoopDevAgent", tools: [...]);
  var evaluator = new CompletionMarkerLoopEvaluator("STATUS: [PASS - VERIFIED]");
  AIAgent loopAgent = new LoopAgent(
      baseAgent,
      evaluator,
      new LoopAgentOptions
      {
          MaxIterations = 5
      });
  ```
- **Rationale**: Exactly follows the Microsoft Learn pattern for MAF LoopAgent.

### Decision 3: Graph Paradigm using `Microsoft.Agents.AI.Workflows.WorkflowBuilder`
- **Choice**: Keep `AgenticWorkflow<T>` backed by `Microsoft.Agents.AI.Workflows.WorkflowBuilder` and `InProcessExecution`.

## Risks / Trade-offs

- **[Risk]** Package dependency resolution during upgrade to 1.13.0.
  - **Mitigation**: Verified packages `Microsoft.Agents.AI 1.13.0` and `Microsoft.Agents.AI.Workflows 1.13.0` are available in NuGet cache.
