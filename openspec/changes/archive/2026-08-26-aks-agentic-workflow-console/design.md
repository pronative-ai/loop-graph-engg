## Context

The application is a .NET 10 console app demonstrating Microsoft Agent Framework (MAF) capabilities. The original code uses Azure Foundry direct connection, but the requirement is to point to an AKS agent gateway URL with environment variable configuration. The application orchestrates multiple AI agents through a directed graph with parallel execution and middleware guardrails.

## Goals / Non-Goals

**Goals:**

- Create a compilable .NET 10 C# 14 console application
- Implement the agentic workflow pattern using MAF primitives
- Configure LLM endpoint to use AKS agent gateway via environment variables
- Provide complete project infrastructure (.gitignore, .aiignore, Makefile, README.md)
- Include .env and .env.example for configuration reference

**Non-Goals:**

- Implement production-ready security (only environment variable configuration)
- Create unit tests (can be added later)
- Implement actual AI agent logic (agents use placeholder implementations)
- Deploy to AKS (only configuration for AKS endpoint)

## Decisions

### Decision: Project Structure

**Choice**: Single console application project with source files in `src/` directory.

**Rationale**: Simple console app doesn't require complex solution structure. Source files organized in `src/` keeps root clean.

**Alternatives considered**:

- Multi-project solution: Overkill for a demonstration app
- Flat structure: Less organized, harder to navigate

### Decision: LLM Client Configuration

**Choice**: Use `AzureOpenAIClient` with `AKS_AGENT_GATEWAY_URL` environment variable instead of Azure Foundry endpoint.

**Rationale**: Matches requirement to use AKS agent gateway. Environment variables allow configuration without code changes.

**Alternatives considered**:

- Hardcode endpoint: Not configurable
- Use configuration files: More complex than needed for console app

### Decision: Middleware Implementation

**Choice**: Implement middleware as async delegates that intercept node transitions.

**Rationale**: Simple and flexible. Allows adding guardrails without modifying core workflow logic.

**Alternatives considered**:

- Attribute-based: Less flexible for runtime decisions
- Event-based: More complex, not needed for this use case

### Decision: Error Handling

**Choice**: Use descriptive exceptions with meaningful messages for configuration and approval failures.

**Rationale**: Clear error messages help users understand what went wrong and how to fix it.

**Alternatives considered**:

- Silent failures: Bad user experience
- Generic exceptions: Hard to debug

## Risks / Trade-offs

[MAF API stability] → The Microsoft Agent Framework may have API changes in future versions. Mitigation: Pin package versions in .csproj file.

[Environment variable dependency] → Application requires environment variables to be set before execution. Mitigation: Validate variables at startup with clear error messages.

[Placeholder implementations] → Agent logic is simplified for demonstration. Mitigation: Document in README that agents use placeholder implementations.

[No unit tests] → Current implementation lacks test coverage. Mitigation: README mentions tests can be added in future iterations.
