## Context

During walkthrough execution, agent turns and LLM calls produced no visible traces in Langfuse because `IChatClient` was not wrapped with `UseOpenTelemetry()`, the required Langfuse v4 ingestion header was omitted, and spans were not explicitly flushed. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Wrap `IChatClient` in `LlmConfiguration.cs` with `.AsBuilder().UseOpenTelemetry(sourceName: "Microsoft.Extensions.AI")`.
- Include `x-langfuse-ingestion-version=4` in OTLP headers when authenticating to Langfuse.
- Provide `TelemetryConfiguration.Flush()` to transmit spans immediately after each walkthrough completes.
- Ensure all tool executions and agent steps generate linked OpenTelemetry spans.

**Non-Goals:**
- Changing LLM model choices or altering business logic.

## Decisions

### Decision 1: Wrap IChatClient with UseOpenTelemetry() Middleware
- *Design*: In `LlmConfiguration.CreateChatClient()`, wrap the raw `AsIChatClient()` adapter with `.AsBuilder().UseOpenTelemetry(sourceName: "Microsoft.Extensions.AI").Build()`.
- *Rationale*: `Microsoft.Extensions.AI` contains built-in OpenTelemetry instrumentation that automatically captures `gen_ai.system`, `gen_ai.request.model`, `gen_ai.usage.input_tokens`, `gen_ai.usage.output_tokens`, and LLM turn spans.

### Decision 2: Automatic Langfuse v4 Header Injection
- *Design*: If Basic Auth is used or Langfuse credentials are provided, append `,x-langfuse-ingestion-version=4` to the OTLP headers if not already present.
- *Rationale*: Langfuse v4 requires this header to parse and display spans properly in the Langfuse web UI.

### Decision 3: Explicit Trace Flushing
- *Design*: Store the active `TracerProvider` and expose `TelemetryConfiguration.Flush()`. Invoke `Flush()` after each walkthrough.
- *Rationale*: Prevents traces from sitting in the in-memory batch exporter queue while the user is at the interactive console menu.

## Risks / Trade-offs

- [Risk] Network latency during span flushing.
  - *Mitigation*: OTLP export uses fast HTTP/protobuf serialization with non-blocking timeouts.
