## Context

The current codebase contains scaffolding and simulation stubs across `LoopAgentDemo.cs`, `GraphWorkflowDemo.cs`, `MiddlewareGuardrail.cs`, and `WorkflowGraph.cs`. While the abstractions (`AgenticWorkflow`, `CodingProjectState`, `ConsoleLogger`, `HumanCheckpointStore`, `TerminalExecutionTool`) are in place, the demos do not currently connect into a unified, live pipeline with genuine tool execution and state passing.

See `proposal.md` for motivation and high-level scope.

## Goals / Non-Goals

**Goals:**
- Implement real autonomous tool-calling loops in `LoopAgentDemo` using `ChatClientAgent` and live diagnostic verification tools.
- Implement end-to-end multi-agent DAG orchestration in `GraphWorkflowDemo` using `AgenticWorkflow<CodingProjectState>` with real agent nodes for Architect, Backend Coder, Frontend Coder, and Code Reviewer.
- Implement genuine middleware interception in `MiddlewareGuardrail` that intercepts the workflow before the `DeploymentNode` and enforces human-in-the-loop authorization.
- Enhance `LlmConfiguration` to robustly instantiate `IChatClient` against OpenAI-compatible gateways with proper endpoint normalization.

**Non-Goals:**
- Building a web UI (this remains a high-fidelity console application).
- Replacing Microsoft Agent Framework with external third-party agent frameworks.
- Deploying actual cloud infrastructure to Kubernetes in demo runs (deployment simulation remains simulated action, but the orchestration and human checkpoint pipeline are real).

## Decisions

### Decision 1: Live Tool Execution via AIFunctionFactory and Terminal/Build Runner
- **Choice**: Register real execution tools with `ChatClientAgent` using `AIFunctionFactory.Create`. In the Loop demo, the agent will analyze actual project status or live code verification diagnostics, proposing modifications and re-verifying until convergence.
- **Rationale**: Demonstrates true agentic autonomy where the model interacts with dynamic environment feedback rather than hardcoded counter flags.
- **Alternative considered**: Mocking the return strings with pre-canned failure/success messages (rejected as requested by user).

### Decision 2: Graph Execution via `AgenticWorkflow<CodingProjectState>`
- **Choice**: Wire `GraphWorkflowDemo` directly to `AgenticWorkflow<CodingProjectState>`:
  1. `ArchitectNode`: Generates architecture document and saves into `state.ArchitectureSpec`.
  2. `ParallelSplit`: Launches `BackendCoderNode` and `FrontendCoderNode` concurrently via `Task.WhenAll`.
  3. `ParallelJoin`: Merges backend and frontend code outputs into `state.GeneratedArtifacts`.
  4. `ReviewerNode`: Evaluates merged code, sets `state.IsApproved = true`.
  5. `ConditionalEdge` / `DeploymentNode`: Transitions to deployment only if approved.
- **Rationale**: Shows the complete power of DAG state transitions, parallel fan-out/join, and conditional branching in Microsoft Agent Framework.
- **Alternative considered**: Sequential single-agent calls with manual print statements (rejected because it lacks graph orchestration).

### Decision 3: Middleware Pipeline for Governance & Checkpoints
- **Choice**: Register a `WorkflowMiddleware` on `AgenticWorkflow` that inspects `context.NextNode`. If `NextNode == "DeploymentNode"`, the middleware pauses, displays the high-visibility console checkpoint prompt with `ConsoleLogger.SecurityWarning`, reads console authorization, and resumes `next()` only when approved.
- **Rationale**: Validates enterprise AI governance patterns by placing security guardrails directly in the execution middleware pipeline.
- **Alternative considered**: Hardcoded `Console.ReadLine()` inside the node method itself (rejected because middleware separation of concerns is a core architectural pattern).

### Decision 4: Endpoint Normalization in `LlmConfiguration`
- **Choice**: Support standard base gateway URLs (like `https://gateway.pronative.ai/v1` or `https://gateway.pronative.ai`) and configure `OpenAIClientOptions` / `IChatClient` cleanly with ApiKeyCredential.
- **Rationale**: Ensures out-of-the-box compatibility with OpenAI-compatible proxies, Azure OpenAI, and custom AI gateways.

## Risks / Trade-offs

- **[Risk]** Slow response times or network timeouts when running multiple LLM agent nodes sequentially/in parallel.
  - **Mitigation**: Use streaming tokens (`RunStreamingAsync`) for immediate visual feedback, and set reasonable token limits / prompt constraints on micro-agents.
- **[Risk]** Missing or invalid `.env` credentials in local environments.
  - **Mitigation**: Provide clear diagnostic messages when environment variables are missing and guide the user on setting up `.env`.
