## Why

The current console demonstration relies on mock methods, simulated build strings, and disconnected agent calls rather than executing full, end-to-end agentic workflows through Microsoft Agent Framework (MAF). Replacing these mocks with robust, production-grade implementations ensures that the Loop Agent autonomous correction, Graph Agent DAG orchestration, and Governance Middleware guardrails operate as fully functional, observable workflows.

## What Changes

- **Real Loop Agent Autonomous Correction**: Replace hardcoded `CompileProject` simulation with a live compilation/verification tool where `ChatClientAgent` genuinely inspects build outputs, diagnoses errors, and performs autonomous iteration.
- **End-to-End DAG Graph Orchestration**: Wire `GraphWorkflowDemo` to execute `AgenticWorkflow<CodingProjectState>` with real agent nodes (`ArchitectAgent`, `BackendCoder`, `FrontendCoder`, `Reviewer`, and `DeploymentNode`), supporting parallel branch execution and state propagation.
- **Integrated Governance Middleware**: Connect `WorkflowMiddleware` and `HumanCheckpointStore` directly into the workflow execution pipeline to intercept deployment transitions, require console operator approval, and enforce human-in-the-loop safety.
- **Robust LLM Gateway Client Integration**: Ensure `LlmConfiguration` initializes `IChatClient` reliably against OpenAI-compatible gateway endpoints with proper error handling and fallback reporting.

## Capabilities

### Modified Capabilities

- `agent-framework-demo`: Update requirements to enforce real agent execution, live tool execution in the loop paradigm, full DAG graph orchestration through `AgenticWorkflow`, and active middleware interception.
- `agentic-workflow-core`: Update requirements to reflect real multi-agent execution with state passing between architect, coders, reviewer, and deployment nodes.
- `workflow-guardrails`: Update requirements to require live middleware interceptor integration with `HumanCheckpointStore` and console approval prompts.

## Impact

- **Affected Components**: `Program.cs`, `LoopParadigm/LoopAgentDemo.cs`, `GraphParadigm/GraphWorkflowDemo.cs`, `Governance/MiddlewareGuardrail.cs`, `WorkflowGraph.cs`, `CodingProjectState.cs`, `LlmConfiguration.cs`.
- **Dependencies**: Uses existing `Microsoft.Agents.AI`, `Microsoft.Extensions.AI`, `Azure.AI.OpenAI`, and `OpenTelemetry.Api`.
- **Breaking Changes**: None; existing console menu choices and CLI interfaces remain compatible while executing real logic.
