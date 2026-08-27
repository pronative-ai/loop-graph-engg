## Why

Vendor-specific naming couples the application configuration to a single backend (Langfuse). Adopting standard OpenTelemetry (OTel) naming conventions (`OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_HEADERS`, `OTEL_SERVICE_NAME`) allows seamless switching between any OTLP-compliant observability platform (such as Langfuse, SigNoz, Jaeger, Honeycomb, or .NET Aspire) without code modifications.

## What Changes

- Standardize `.env.example` on OpenTelemetry official specification environment variables:
  - `OTEL_EXPORTER_OTLP_ENDPOINT=https://dev-monitoring.pronative.ai/api/public/otel`
  - `OTEL_EXPORTER_OTLP_HEADERS=Authorization=Basic <base64(pk:sk)>`
  - `OTEL_SERVICE_NAME=loop-vs-graph`
- Update `src/Shared/TelemetryConfiguration.cs` to prioritize standard OTel environment variables (`OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_HEADERS`, `OTEL_SERVICE_NAME`) across all tracer initialization logic.
- Update `tests/AksAgenticWorkflowConsole.Tests/TelemetryConfigurationTests.cs` to test standard OTel variable resolution and header forwarding.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `env-config`: Adopt vendor-neutral OpenTelemetry standard environment variable names (`OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_HEADERS`, `OTEL_SERVICE_NAME`).

## Impact

- **`.env.example`**: Standard OpenTelemetry configuration section with vendor-agnostic instructions.
- **`src/Shared/TelemetryConfiguration.cs`**: Vendor-neutral OTLP configuration.
- **`tests/AksAgenticWorkflowConsole.Tests/TelemetryConfigurationTests.cs`**: Updated unit tests.
