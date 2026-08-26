# Proposal: Cleanup Environment Variables and Comments

## Summary

Remove verbose XML documentation comments from methods and classes, simplify environment variable naming by removing the `AKS_` prefix, and update the endpoint configuration to use a cleaner gateway URL / model name pattern.

## Motivation

- **Comment noise**: XML doc comments on every method and class add visual clutter without value in a small console app. They duplicate what the code already communicates.
- **Prefix redundancy**: The `AKS_` prefix on environment variables (`AKS_AGENT_GATEWAY_URL`, `AKS_AGENT_GATEWAY_KEY`, `AKS_MODEL_NAME`) is project-specific noise. These are just gateway URL, gateway key, and model name.
- **Endpoint simplification**: The endpoint should be constructed as `gateway_url / model_name` rather than treating them as separate configuration concerns.

## Scope

- `.env` and `.env.example`: Rename variables, remove `AKS_` prefix
- `src/LlmConfiguration.cs`: Update variable references, simplify endpoint logic
- `src/Program.cs`: Update variable references in validation and config output
- All `.cs` files: Remove XML documentation comments from methods and classes

## Out of Scope

- Functional behavior changes (same external behavior, cleaner internals)
- New features or capabilities
- Test changes (tests should continue passing with updated variable names)

## Acceptance Criteria

- Environment variables use names: `GATEWAY_URL`, `GATEWAY_KEY`, `MODEL_NAME`
- All XML documentation comments removed from methods and classes
- Endpoint constructed as `{GATEWAY_URL}/{MODEL_NAME}` pattern
- Application starts correctly with new variable names
- `.env.example` updated to reflect new variable names