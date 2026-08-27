## 1. Clean & Resilient Telemetry Configuration

- [x] 1.1 Update `src/Shared/TelemetryConfiguration.cs` with quote sanitization, endpoint `/v1/traces` normalization, and removal of legacy `LANGFUSE_*` lookups
- [x] 1.2 Instrument `src/WorkflowGraph.cs` with OpenTelemetry activity spans for all graph node executions
- [x] 1.3 Add `TelemetryConfiguration.Flush()` to `src/GraphParadigm/GraphWorkflowWalkthrough.cs` and `src/Governance/MiddlewareGuardrail.cs`
- [x] 1.4 Validate and verify `.env.example` guidance

## 2. Unit Testing & Verification

- [x] 2.1 Update `tests/AksAgenticWorkflowConsole.Tests/TelemetryConfigurationTests.cs` to test quote stripping, endpoint normalization, and pure standard OTel variables
- [x] 2.2 Run `make test` and confirm all tests pass
