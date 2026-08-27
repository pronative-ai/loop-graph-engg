## ADDED Requirements

### Requirement: OpenTelemetry observability

The system SHALL initialize OpenTelemetry distributed tracing and export all agent runs, LLM requests, tool invocations, and workflow transitions to the configured Langfuse OTLP endpoint.

#### Scenario: OpenTelemetry tracer initialization

- **WHEN** the application starts
- **THEN** the system configures an OpenTelemetry `TracerProvider` subscribing to Microsoft Agent Framework (`Microsoft.Agents.AI.*`), Microsoft Extensions AI (`Microsoft.Extensions.AI.*`), and application activity sources

#### Scenario: Langfuse OTLP export

- **WHEN** traces and spans are recorded during agent execution
- **THEN** the system securely exports spans via OTLP to `https://dev-monitoring.pronative.ai/api/public/otel` using Langfuse credentials

#### Scenario: Trace lifecycle management

- **WHEN** the application shuts down or completes a walkthrough
- **THEN** all pending OpenTelemetry trace spans are flushed before process exit
