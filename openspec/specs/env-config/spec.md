## Purpose

Simplify environment variable naming and endpoint configuration for the LLM gateway connection.

## Requirements

### Requirement: Environment variable configuration

The system SHALL read LLM endpoint and OpenTelemetry distributed tracing configuration from environment variables using official OpenTelemetry standard names.

#### Scenario: Load gateway URL from environment

- **WHEN** the application starts
- **THEN** the system reads `GATEWAY_URL` environment variable and uses it as the LLM endpoint

#### Scenario: Load gateway key from environment

- **WHEN** the application starts
- **THEN** the system reads `GATEWAY_KEY` environment variable and uses it for authentication

#### Scenario: Load model name from environment

- **WHEN** the application starts
- **THEN** the system reads `MODEL_NAME` environment variable and uses it for model selection

#### Scenario: Load standard OpenTelemetry monitoring configuration

- **WHEN** the application starts
- **THEN** the system reads `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_HEADERS`, and `OTEL_SERVICE_NAME` to route traces to any OTLP collector (such as Langfuse or SigNoz)

#### Scenario: Missing environment variables

- **WHEN** required environment variables are not set
- **THEN** the system throws a descriptive error indicating which variable is missing

### Requirement: Endpoint construction

The system SHALL construct the LLM endpoint by combining the gateway URL and model name.

#### Scenario: Construct endpoint from configuration

- **WHEN** the gateway URL and model name are loaded
- **THEN** the system constructs the endpoint as `{GATEWAY_URL}/{MODEL_NAME}`

#### Scenario: Default model fallback

- **WHEN** `MODEL_NAME` is not set
- **THEN** the system defaults to `gpt-4o`

### Requirement: Code documentation

The system SHALL use minimal code documentation without verbose XML comments on methods and classes.

#### Scenario: Remove XML documentation comments

- **WHEN** source code is reviewed
- **THEN** methods and classes do not have XML documentation comment blocks
