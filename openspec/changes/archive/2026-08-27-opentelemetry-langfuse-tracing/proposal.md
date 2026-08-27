## Why

To achieve production-grade observability and monitoring for all AI agent interactions, the application needs to integrate OpenTelemetry (OTel) and export traces to Langfuse (hosted at `https://dev-monitoring.pronative.ai`). All agent runs, LLM calls, tool executions, and workflow transitions must be captured and traced automatically via OTLP.

## What Changes

- Add OpenTelemetry packages (`OpenTelemetry`, `OpenTelemetry.Exporter.OpenTelemetryProtocol`, `OpenTelemetry.Extensions.Hosting`) to `src/AksAgenticWorkflowConsole.csproj`.
- Implement `TelemetryConfiguration.cs` in `src/Shared/` to configure the OpenTelemetry `TracerProvider`:
  - Listen to activity sources for `Microsoft.Agents.AI.*`, `Microsoft.Extensions.AI.*`, and application-level activity sources.
  - Automatically configure the OTLP exporter to send traces to `https://dev-monitoring.pronative.ai/api/public/otel` with Langfuse authentication headers.
- Update `.env.example` with the required Langfuse and OpenTelemetry environment variables:
  - `LANGFUSE_HOST=https://dev-monitoring.pronative.ai`
  - `LANGFUSE_PUBLIC_KEY=pk-lf-...`
  - `LANGFUSE_SECRET_KEY=sk-lf-...`
  - `OTEL_EXPORTER_OTLP_ENDPOINT=https://dev-monitoring.pronative.ai/api/public/otel`
- Integrate OpenTelemetry lifecycle in `src/Program.cs` to initialize tracing on startup and flush traces cleanly on exit.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `env-config`: Add OpenTelemetry and Langfuse configuration variables (`LANGFUSE_HOST`, `LANGFUSE_PUBLIC_KEY`, `LANGFUSE_SECRET_KEY`, `OTEL_EXPORTER_OTLP_ENDPOINT`).
- `agent-framework-demo`: Add OpenTelemetry distributed tracing requirements for all agent activities, LLM turns, and tool calls.

## Impact

- **`src/AksAgenticWorkflowConsole.csproj`**: Adds OpenTelemetry exporter packages.
- **`src/Shared/TelemetryConfiguration.cs`**: New telemetry setup and OTLP exporter initialization.
- **`src/Program.cs`**: Initializes and flushes OpenTelemetry tracer provider.
- **`.env.example`**: Documents Langfuse and OpenTelemetry credentials.
