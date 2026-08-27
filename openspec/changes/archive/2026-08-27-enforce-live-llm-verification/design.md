## Context

The system requires that all code validation strictly executes against a live `IChatClient` without mock or offline fallback strings. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Update `LoopDiagnosticWorkspace.CompileAndVerifyAsync(IChatClient chatClient)` to require a non-null `IChatClient`.
- Remove any return branches containing "Offline build check".
- Update unit tests in `LoopDiagnosticTests.cs` to test genuine `IChatClient` interaction and null argument validation.

**Non-Goals:**
- External network requirements in unit test fixtures (use in-memory `IChatClient` delegates for deterministic test isolation).

## Decisions

### Decision 1: Require `IChatClient` parameter
- *Design*: Make `IChatClient chatClient` mandatory in `LoopDiagnosticWorkspace.CompileAndVerifyAsync(IChatClient chatClient)`. If null, throw `ArgumentNullException`.
- *Rationale*: Eliminates dead code paths and guarantees that the system always evaluates live LLM output.

### Decision 2: Unit test with `IChatClient` delegate
- *Design*: In unit tests, provide a lightweight test `IChatClient` implementation that simulates real evaluator returns (e.g. `STATUS: [FAIL]`, `STATUS: [PASS - VERIFIED]`) through the standard MAF `IChatClient.GetResponseAsync` pipeline.
- *Rationale*: Validates the full parsing, state tracking, and iteration incrementation logic across `IChatClient` calls.

## Risks / Trade-offs

- [Risk] Null chat client passed during unconfigured runtime.
  - *Mitigation*: `LoopAgentWalkthrough.RunAsync` already checks `if (baseClient == null)` and prints a clear configuration warning before invoking the workspace.
