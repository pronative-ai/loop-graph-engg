## MODIFIED Requirements

### Requirement: Environment variable configuration

The system SHALL read LLM endpoint and OpenTelemetry / Langfuse monitoring configuration from environment variables.

#### Scenario: Load gateway URL from environment

- **WHEN** the application starts
- **THEN** the system reads `GATEWAY_URL` environment variable and uses it as the LLM endpoint

#### Scenario: Load gateway key from environment

- **WHEN** the application starts
- **THEN** the system reads `GATEWAY_KEY` environment variable and uses it for authentication

#### Scenario: Load model name from environment

- **WHEN** the application starts
- **THEN** the system reads `MODEL_NAME` environment variable and uses it for model selection

#### Scenario: Load OpenTelemetry and Langfuse monitoring variables

- **WHEN** the application starts
- **THEN** the system loads `LANGFUSE_HOST`, `LANGFUSE_PUBLIC_KEY`, `LANGFUSE_SECRET_KEY`, and `OTEL_EXPORTER_OTLP_ENDPOINT` for trace export

#### Scenario: Missing environment variables

- **WHEN** required environment variables are not set
- **THEN** the system throws a descriptive error indicating which variable is missing
