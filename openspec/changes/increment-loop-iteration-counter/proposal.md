## Why

During the Loop Engineering Walkthrough, tool calls and observations inside the agent's autonomous execution were not incrementing the iteration counter `s_iteration` on each verification/patch cycle, resulting in multiple distinct loop cycles displaying the same iteration number (e.g. `[Loop #1]`). The iteration counter must increment on each verification/diagnostic cycle so that students see the iteration number progressively advance (`[Loop #1]`, `[Loop #2]`, `[Loop #3]`).

## What Changes

- Bind the active display iteration counter directly to `workspace.IterationCount` (or increment `s_iteration` dynamically on each `CompileAndVerify` / diagnostic evaluation cycle).
- Ensure all headers (`[Loop #X] [LLM REASONING]`, `[Loop #X] [TOOL CALL]`, `[Loop #X] [OBSERVATION]`) always display the dynamically incremented iteration number.
- Ensure outer loop synchronization and final summary accurately reflect the total number of executed iterations (e.g. `Completed successfully in 3 iterations!`).

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agent-framework-demo`: Ensure each loop cycle and verification invocation dynamically increments and displays the iteration counter.

## Impact

- **`src/LoopParadigm/LoopAgentWalkthrough.cs`**: Synchronize iteration counter with each verification invocation so terminal logs show increasing loop numbers (`[Loop #1]`, `[Loop #2]`, `[Loop #3]`).
