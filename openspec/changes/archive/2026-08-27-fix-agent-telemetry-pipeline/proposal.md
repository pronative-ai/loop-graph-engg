## Why

No telemetry data was received in Langfuse because the underlying `IChatClient` pipeline in `LlmConfiguration.cs` was not wrapped with `UseOpenTelemetry()`, meaning `Microsoft.Extensions.AI` never emitted `gen_ai.*` activity spans. In addition, the OTLP exporter lacked the required Langfuse v4 ingestion header (`x-langfuse-ingestion-version=4`) and explicit walkthrough span flushing.

## What Changes

- Wrap `IChatClient` in `src/LlmConfiguration.cs` with `.AsBuilder().UseOpenTelemetry(sourceName: "Microsoft.Extensions.AI").Build()` to instrument all LLM turns, token counts, and completions.
- Enhance `src/Shared/TelemetryConfiguration.cs`:
  - Automatically attach `x-langfuse-ingestion-version=4` header when targeting Langfuse or exporting OTLP headers.
  - Correctly normalize OTLP HTTP exporter endpoints for .NET OpenTelemetry (`https://dev-monitoring.pronative.ai/api/public/otel`).
  - Provide helper `Flush()` to flush pending spans immediately after each agent walkthrough execution.
- Instrument tool executions and agent walkthroughs with OpenTelemetry activity spans and tags (`gen_ai.agent.name`, `loop.iteration`, `tool.name`).
- Update unit tests to verify `UseOpenTelemetry` wrapping and telemetry header configuration.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agent-framework-demo`: Ensure all agent operations, LLM calls, and tool activities generate active OpenTelemetry spans and transmit immediately to the OTLP collector.

## Impact

- **`src/LlmConfiguration.cs`**: Instruments `IChatClient` with `UseOpenTelemetry()`.
- **`src/Shared/TelemetryConfiguration.cs`**: Appends `x-langfuse-ingestion-version=4` and ensures reliable OTLP HTTP trace transmission.
- **`src/Program.cs` / Walkthroughs**: Flushes telemetry traces after each walkthrough execution.
- **`tests/AksAgenticWorkflowConsole.Tests/`**: Adds verification tests.
