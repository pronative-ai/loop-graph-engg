## MODIFIED Requirements

### Requirement: Environment variable configuration

The system SHALL read LLM endpoint and OpenTelemetry distributed tracing configuration from environment variables using official OpenTelemetry standard names, sanitizing any surrounding quotation marks and normalizing OTLP signal endpoints.

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
- **THEN** the system reads and sanitizes `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_HEADERS`, and `OTEL_SERVICE_NAME` to route traces to any OTLP collector (such as Langfuse or SigNoz)

#### Scenario: Normalize OTLP HTTP trace endpoint path

- **WHEN** the OpenTelemetry tracer is initialized with an endpoint containing a path prefix
- **THEN** the system resolves the explicit full trace endpoint ending with `/v1/traces` to prevent relative URI truncation by the OTLP HTTP exporter

#### Scenario: Subscribe to all application activity sources

- **WHEN** the OpenTelemetry tracer provider is built
- **THEN** the system subscribes to all activity sources via wildcard and framework namespaces to ensure complete trace capture

#### Scenario: Missing environment variables

- **WHEN** required environment variables are not set
- **THEN** the system throws a descriptive error indicating which variable is missing
