## 1. OpenTelemetry Standardization

- [x] 1.1 Update `.env.example` to use standard OpenTelemetry environment variables (`OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_HEADERS`, `OTEL_SERVICE_NAME`) with multi-backend guidance (Langfuse, SigNoz)
- [x] 1.2 Update `src/Shared/TelemetryConfiguration.cs` to prioritize standard OTel variables and service naming

## 2. Test Suite & Verification

- [x] 2.1 Update `tests/AksAgenticWorkflowConsole.Tests/TelemetryConfigurationTests.cs` to test standard OTel endpoint and header loading
- [x] 2.2 Run `make test` and confirm all tests pass
