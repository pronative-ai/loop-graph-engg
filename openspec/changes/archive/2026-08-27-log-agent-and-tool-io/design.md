## Context

See `proposal.md` for motivation. The solution uses Microsoft Agent Framework (`Microsoft.Agents.AI`) and `Microsoft.Extensions.AI` with an OpenTelemetry trace pipeline targeting Langfuse. Currently, Activity spans exist for loops and nodes, but tool arguments and node payload transformations are not recorded as explicit span tags and events.

## Goals / Non-Goals

**Goals:**
- Record all tool input parameters (`filePath`, `patchContent`, `buildTarget`) and return values as span attributes (`gen_ai.tool.input`, `gen_ai.tool.output`, `gen_ai.tool.status`).
- Record all workflow graph node inputs (`workflow.node.input_spec`, `workflow.node.goal`) and outputs (`workflow.node.output_artifacts`, `workflow.node.logs`) on `Workflow.Node.<Name>` spans.
- Enhance `ConsoleLogger` with structured helpers to cleanly render inputs/outputs during interactive sessions without visual clutter.
- Record human operator guardrail decisions (`guardrail.operator_action`, `guardrail.session_id`) on governance spans.

**Non-Goals:**
- Altering the core business logic of coding agents or DAG transitions.
- Adding third-party logging frameworks (maintaining standard .NET `System.Diagnostics.Activity` and `ConsoleLogger`).

## Decisions

### Decision 1: Standardized Activity Tag Attribute Names
- **Choice**: Use OpenTelemetry Semantic Conventions for GenAI / Agent tools:
  - `gen_ai.tool.name`: Tool identifier (`InspectCode`, `ApplyCodeFix`, `CompileAndVerify`).
  - `gen_ai.tool.input`: Serialized input arguments or key parameters.
  - `gen_ai.tool.output`: String representation of the result or compiler verdict.
  - `gen_ai.tool.is_success`: Boolean flag indicating tool success or failure.
- **Rationale**: Complies with standard OTel collector indexing (Langfuse, SigNoz, Jaeger).

### Decision 2: State Tracking in Workflow Graph Nodes
- **Choice**: In `WorkflowGraph.cs`, capture snapshot before `currentNode.ExecuteAsync(state)` and compare after to tag changes.
- **Rationale**: Avoids intrusive coupling inside agent instruction strings.

## Risks / Trade-offs

- **Large Payload Ingestion in Tracing**: Large code buffers might increase trace payload sizes.
  - *Mitigation*: Truncate attributes to reasonable preview lengths (e.g. 2048 chars) if payloads exceed limits, while preserving full console output.
