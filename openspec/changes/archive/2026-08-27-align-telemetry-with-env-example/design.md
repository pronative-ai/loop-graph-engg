## Context

The configuration variables in `.env` often contain surrounding quotes (e.g., `OTEL_EXPORTER_OTLP_HEADERS="Authorization=Basic ...,x-langfuse-ingestion-version=4"`), which if passed directly to the .NET OTLP exporter, corrupt the HTTP request headers. Additionally, DAG workflow executions were missing parent spans and explicit flush triggers. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Strip enclosing double and single quotes from all environment variables loaded by `TelemetryConfiguration`.
- Normalize endpoint URLs to avoid accidental duplication of `/v1/traces`.
- Instrument `WorkflowGraph.cs` to emit activity spans for all DAG workflow nodes.
- Ensure `TelemetryConfiguration.Flush()` is executed in `finally` blocks for `GraphWorkflowWalkthrough` and `MiddlewareGuardrail`.
- Remove legacy `LANGFUSE_*` fallback keys.

**Non-Goals:**
- Changing external OpenTelemetry export wire protocol or SDK dependencies.

## Decisions

### Decision 1: Quote Stripping and Header Sanitization
- *Design*: Implement `.Trim('"', '\'', ' ')` on environment variable values before parsing or passing to `OpenTelemetry.Exporter.OtlpExporterOptions`.
- *Rationale*: Protects against malformed headers when values in `.env` are quoted.

### Decision 2: Endpoint Path Normalization
- *Design*: If the configured endpoint already ends in `/v1/traces`, strip the trailing `/v1/traces` when passing to `OtlpExportProtocol.HttpProtobuf` options so that the .NET exporter generates the exact route `https://<host>/api/public/otel/v1/traces`.
- *Rationale*: Prevents duplicate route segments like `/api/public/otel/v1/traces/v1/traces`.

### Decision 3: Graph and Guardrail Activity Instrumentation
- *Design*: Wrap every `WorkflowNode.ExecuteAsync` in `WorkflowGraph.cs` with `TelemetryConfiguration.ActivitySource.StartActivity("Workflow.Node.<Name>")`.
- *Rationale*: Gives complete end-to-end visibility of all multi-agent graph flows in Langfuse/SigNoz.

## Risks / Trade-offs

- [Risk] Trace export timeouts on slow networks.
  - *Mitigation*: `TelemetryConfiguration.Flush()` runs with a 5000ms timeout and catches transient exceptions safely.
