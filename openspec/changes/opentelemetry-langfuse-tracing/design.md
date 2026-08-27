## Context

The application executes autonomous agents and multi-agent workflows using Microsoft Agent Framework (MAF). Observability is required to monitor token usage, latency, tool calls, and execution steps by exporting OpenTelemetry traces to a self-hosted Langfuse instance at `https://dev-monitoring.pronative.ai`. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Configure OpenTelemetry trace collection for all MAF agents, tools, and workflows.
- Export traces over OTLP HTTP/protobuf to Langfuse with HTTP Basic authentication (`Authorization: Basic <base64(pk:sk)>`).
- Provide clean configuration via `.env` and document variables in `.env.example`.
- Ensure tracing runs gracefully and does not block execution if monitoring credentials are not set.

**Non-Goals:**
- Modifying core graph algorithms or agent prompts.

## Decisions

### Decision 1: Use OpenTelemetry .NET SDK with OTLP Exporter
- *Design*: Add `OpenTelemetry` and `OpenTelemetry.Exporter.OpenTelemetryProtocol` packages, configuring `TracerProviderBuilder` with `AddOtlpExporter`.
- *Rationale*: Industry standard for distributed tracing; natively supported by both Microsoft Agent Framework and Langfuse.

### Decision 2: Automatic Langfuse Basic Auth construction
- *Design*: `TelemetryConfiguration` checks `LANGFUSE_PUBLIC_KEY` and `LANGFUSE_SECRET_KEY`. If provided, it generates the required OTLP `Authorization: Basic {base64}` header automatically.
- *Rationale*: Eliminates manual base64 encoding errors for developers and aligns with Langfuse's OpenTelemetry specification.

### Decision 3: Capture MAF and Custom Activity Sources
- *Design*: Register ActivitySources:
  - `Microsoft.Agents.AI` / `Microsoft.Agents.AI.*`
  - `Microsoft.Extensions.AI` / `Microsoft.Extensions.AI.*`
  - `AgenticWorkflowConsole` / `AgenticWorkflowConsole.*`
- *Rationale*: Ensures full end-to-end visibility: agent reasoning, LLM completion calls, tool invocations, and workflow graph nodes.

## Risks / Trade-offs

- [Risk] Missing Langfuse credentials in local environments.
  - *Mitigation*: Gracefully log a telemetry initialization notice and continue local execution without throwing exceptions.
