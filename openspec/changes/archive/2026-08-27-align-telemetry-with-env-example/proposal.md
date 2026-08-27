## Why

OpenTelemetry data was not reaching the Langfuse/SigNoz collector because:
1. Surrounding quotation marks in `.env` values (e.g. `OTEL_EXPORTER_OTLP_HEADERS="Authorization=Basic ..."`) were not sanitized, resulting in malformed HTTP request headers.
2. Endpoint URL normalization did not guard against duplicate `/v1/traces` subpaths in the .NET OTLP HttpProtobuf exporter.
3. Graph workflow and governance walkthrough nodes did not create OpenTelemetry child activity spans or invoke `TelemetryConfiguration.Flush()` upon completion.

## What Changes

- **Header and Endpoint Sanitization**:
  - Strip leading/trailing quotation marks (`"`, `'`) and whitespace from `OTEL_EXPORTER_OTLP_HEADERS`, `OTEL_EXPORTER_OTLP_ENDPOINT`, and `OTEL_SERVICE_NAME`.
  - Normalize OTLP endpoint URLs so that `.NET`'s OTLP HttpProtobuf exporter targets the exact traces route without duplicating `/v1/traces`.
- **Comprehensive Activity Instrumentation**:
  - Add OpenTelemetry activity spans to `WorkflowGraph.cs` for all workflow node executions (`Workflow.Node.<NodeName>`).
  - Add `TelemetryConfiguration.Flush()` to `GraphWorkflowWalkthrough.cs` and `MiddlewareGuardrail.cs` in `finally` blocks.
- **Strict Clean Configuration**:
  - Remove all legacy `LANGFUSE_*` fallback lookups from `TelemetryConfiguration.cs` and test files.
- **Unit Testing**:
  - Add unit tests verifying quotation sanitization, endpoint normalization, and trace flushing.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `env-config`: Ensure environment variables (`OTEL_EXPORTER_OTLP_HEADERS`, `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_SERVICE_NAME`) are sanitized, robustly parsed, and exported without legacy vendor prefixes.

## Impact

- **`src/Shared/TelemetryConfiguration.cs`**: Robust header parsing, quote stripping, endpoint normalization.
- **`src/WorkflowGraph.cs`**: Workflow node activity spans.
- **`src/GraphParadigm/GraphWorkflowWalkthrough.cs`**: Activity tracing and flush.
- **`src/Governance/MiddlewareGuardrail.cs`**: Activity tracing and flush.
- **`tests/AksAgenticWorkflowConsole.Tests/TelemetryConfigurationTests.cs`**: Unit tests.
