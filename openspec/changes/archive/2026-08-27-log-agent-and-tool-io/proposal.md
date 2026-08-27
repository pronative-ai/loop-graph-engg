## Why

In enterprise agentic systems and Langfuse observability dashboards, inspecting agents and tools requires full input arguments, prompt payloads, model generations, and tool return values attached directly to trace spans and logs. Currently, tool spans and workflow node spans only record top-level identifiers without comprehensive input/output payloads attached as span attributes and console traces.

## What Changes

- **Comprehensive Tool Input & Output Recording**: Instrument `InspectCode`, `ApplyCodeFix`, and `CompileAndVerify` tools to attach input arguments (`filePath`, `patchContent`, `diagnosticGoal`) and return values (`inspectionContent`, `fixResult`, `verificationLogs`) as attributes on their OpenTelemetry spans (`gen_ai.tool.input`, `gen_ai.tool.output`, `gen_ai.tool.error`).
- **Workflow Node Input & Output Recording**: Instrument DAG workflow nodes (`ArchitectNode`, `BackendCoderNode`, `FrontendCoderNode`, `ReviewerNode`, `DeploymentNode`) to capture incoming state specifications as input tags and resulting state outputs as span attributes.
- **Agent Reasoning Input & Output Streaming**: Ensure streaming prompt updates and tool responses are recorded with full visibility in console and OpenTelemetry traces.
- **Governance Checkpoint Audit Logging**: Record operator inputs, approvals, and rejections directly on guardrail trace spans (`guardrail.operator_action`, `guardrail.session_id`, `guardrail.reason`).

## Capabilities

### Modified Capabilities

- `agent-framework-demo`: Add requirement scenarios for recording complete agent, workflow node, and tool execution inputs and outputs to OpenTelemetry spans and structured logs.

## Impact

- Affected files:
  - `src/LoopParadigm/LoopAgentWalkthrough.cs` (Tool input/output span tags and structured logging)
  - `src/GraphParadigm/GraphWorkflowWalkthrough.cs` (Agent node prompt and output propagation logging)
  - `src/WorkflowGraph.cs` (Node input/output span attributes)
  - `src/Governance/MiddlewareGuardrail.cs` (Guardrail decision audit attributes)
  - `tests/AksAgenticWorkflowConsole.Tests/` (Unit tests verifying telemetry tag capture and tool payload logging)
