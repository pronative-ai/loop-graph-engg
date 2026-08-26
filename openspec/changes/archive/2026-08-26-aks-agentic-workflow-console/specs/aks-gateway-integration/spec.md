## Purpose

Connect LLM clients to AKS agent gateway endpoint using environment variable configuration instead of Azure Foundry direct connections.

## ADDED Requirements

### Requirement: Environment variable configuration

The system SHALL read LLM endpoint configuration from environment variables.

#### Scenario: Load gateway URL from environment

- **WHEN** the application starts
- **THEN** the system reads `AKS_AGENT_GATEWAY_URL` environment variable and uses it as the LLM endpoint

#### Scenario: Load gateway key from environment

- **WHEN** the application starts
- **THEN** the system reads `AKS_AGENT_GATEWAY_KEY` environment variable and uses it for authentication

#### Scenario: Missing environment variables

- **WHEN** required environment variables are not set
- **THEN** the system throws a descriptive error indicating which variable is missing

### Requirement: AKS agent gateway client creation

The system SHALL create LLM clients configured to communicate with the AKS agent gateway.

#### Scenario: Create configured client

- **WHEN** environment variables are loaded
- **THEN** the system creates an `AzureOpenAIClient` pointing to the gateway URL with appropriate authentication

#### Scenario: Create chat client

- **WHEN** the LLM client is created
- **THEN** the system creates a chat client for model inference operations

### Requirement: Model selection configuration

The system SHALL support configurable model selection for agent operations.

#### Scenario: Default model usage

- **WHEN** no model override is specified
- **THEN** the system uses the default configured model (e.g., gpt-4o)

#### Scenario: Custom model selection

- **WHEN** a specific model is required
- **THEN** the system allows specifying the model name at client creation time
