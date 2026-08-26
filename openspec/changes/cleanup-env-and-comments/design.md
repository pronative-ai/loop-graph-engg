# Design: Cleanup Environment Variables and Comments

## Approach

Straightforward refactoring with no architectural changes. Each file is modified independently.

## Environment Variable Renaming

| Current | New |
|---------|-----|
| `AKS_AGENT_GATEWAY_URL` | `GATEWAY_URL` |
| `AKS_AGENT_GATEWAY_KEY` | `GATEWAY_KEY` |
| `AKS_MODEL_NAME` | `MODEL_NAME` |

Update locations:
- `.env` and `.env.example`
- `src/LlmConfiguration.cs` (3 occurrences)
- `src/Program.cs` (validation array)

## Endpoint Construction

Modify `LlmConfiguration.CreateClient()` to accept an optional model name and construct the endpoint as `{gatewayUrl}/{modelName}`. The `AzureOpenAIClient` is created with this combined endpoint.

## Comment Cleanup

Remove all `/// <summary>`, `/// <param>`, `/// <returns>`, `/// <exception>`, and `/// <typeparam>` XML doc comment blocks from:
- `src/LlmConfiguration.cs`
- `src/Program.cs`
- `src/WorkflowGraph.cs`
- `src/TerminalExecutionTool.cs`
- `src/HumanCheckpointStore.cs`
- `src/CodingProjectState.cs`

## Files Modified

- `.env` - rename variables
- `.env.example` - rename variables, update comments
- `src/LlmConfiguration.cs` - rename variables, update endpoint, remove XML docs
- `src/Program.cs` - rename variables in validation, remove XML docs
- `src/WorkflowGraph.cs` - remove XML docs
- `src/TerminalExecutionTool.cs` - remove XML docs
- `src/HumanCheckpointStore.cs` - remove XML docs
- `src/CodingProjectState.cs` - remove XML docs