## 1. Dependencies & Telemetry Implementation

- [x] 1.1 Add `OpenTelemetry` and `OpenTelemetry.Exporter.OpenTelemetryProtocol` package references to `src/AksAgenticWorkflowConsole.csproj`
- [x] 1.2 Implement `src/Shared/TelemetryConfiguration.cs` with OTLP exporter, Langfuse Basic Auth header generation, and ActivitySource subscriptions
- [x] 1.3 Update `.env.example` with Langfuse and OpenTelemetry configuration variables
- [x] 1.4 Integrate `TelemetryConfiguration` lifecycle into `src/Program.cs` to capture and flush distributed traces

## 2. Verification & Validation

- [x] 2.1 Add unit tests in `tests/AksAgenticWorkflowConsole.Tests/TelemetryConfigurationTests.cs` to verify telemetry endpoint and authentication header generation
- [x] 2.2 Run `make test` and confirm all tests pass
