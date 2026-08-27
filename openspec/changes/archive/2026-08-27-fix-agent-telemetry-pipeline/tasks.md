## 1. Telemetry Pipeline Implementation

- [x] 1.1 Wrap `IChatClient` in `src/LlmConfiguration.cs` with `.AsBuilder().UseOpenTelemetry(sourceName: "Microsoft.Extensions.AI").Build()`
- [x] 1.2 Update `src/Shared/TelemetryConfiguration.cs` to inject `x-langfuse-ingestion-version=4` and provide `TelemetryConfiguration.Flush()`
- [x] 1.3 Add walkthrough and tool execution activity spans and invoke `TelemetryConfiguration.Flush()` after each run in `src/Program.cs` and `src/LoopParadigm/LoopAgentWalkthrough.cs`

## 2. Verification & Validation

- [x] 2.1 Add unit tests in `tests/AksAgenticWorkflowConsole.Tests/TelemetryConfigurationTests.cs` verifying ingestion header generation and trace provider flushing
- [x] 2.2 Run `make test` and confirm all tests pass
