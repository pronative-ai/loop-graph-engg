## Context

During autonomous loop execution, tool calls and observations need to reflect the dynamically incrementing iteration count as the agent inspects, patches, and re-compiles the code. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Dynamically increment and synchronize the iteration counter with `workspace.IterationCount` during live tool execution (`CompileAndVerify`, `ApplyCodeFix`, `InspectCode`).
- Display progressive loop headers: `[Loop #1]`, `[Loop #2]`, `[Loop #3]` across successive reasoning and tool evaluation cycles.
- Accurately report total completed iterations upon convergence.

**Non-Goals:**
- Changing LLM model endpoints or system prompts unrelated to iteration tracking.

## Decisions

### Decision 1: Derive Display Iteration from `workspace.IterationCount`
- *Design*: Compute active iteration directly from `workspace.IterationCount` in tools and outer loop. When `CompileAndVerify` executes, it uses `workspace.IterationCount + 1`, and `workspace.IterationCount` increments on every evaluation pass.
- *Rationale*: Guarantees that every compile/verification attempt advances the visible loop iteration counter naturally.

## Risks / Trade-offs

- [Risk] In-turn tool calls prior to first compile.
  - *Mitigation*: Fall back to `Math.Max(1, workspace.IterationCount)` so initial inspect/patch before compile shows `[Loop #1]`.
