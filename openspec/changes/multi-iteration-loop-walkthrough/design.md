## Context

Students and attendees need to observe how an autonomous AI Agent in Microsoft Agent Framework (MAF) continuously loops to solve a real coding problem through iterative feedback: detecting a compiler error -> fixing it -> observing a compiler warning -> fixing it -> achieving clean verification. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Implement a stateful `LoopVerificationEngine` / `DiagnosticSession` that provides realistic, progressive compiler diagnostics across 3-4 loop iterations:
  - Phase 1: Critical compilation error (`CS0103: The name 'ApplyTierDiscount' does not exist in current context`).
  - Phase 2: Compiler warning (`CS8602: Dereference of a possibly null reference 'customer'`).
  - Phase 3/4: Clean compilation (`0 Warning(s), 0 Error(s), 100% Tests Passed`).
- Register real MAF tools with `AIFunctionFactory`:
  - `InspectCode`: Retrieves current code file and diagnostic summary.
  - `ApplyCodeFix`: Applies code corrections and patches.
  - `CompileAndVerify`: Runs live build verification and returns compiler outputs.
- Execute real streaming LLM calls using `ChatClientAgent` across iterations, displaying colorized headers:
  - `[Loop #X] [LLM REASONING]` (Blue)
  - `[Loop #X] [TOOL CALL]` (Cyan)
  - `[Loop #X] [OBSERVATION]` (DarkGray)
- Guarantee that the loop executes at least 2 to 4 iterations before convergence.

**Non-Goals:**
- Modifying the Graph Engineering Walkthrough or Governance Middleware.

## Decisions

### Decision 1: Progressive Diagnostic Engine for Deterministic Multi-Step Learning
- *Design*: Create `LoopDiagnosticWorkspace` in `src/LoopParadigm/` that holds the target code file and validates fixes across stages (Error -> Warning -> Clean).
- *Rationale*: A real live `dotnet build` on the host console repo itself may already be in a clean state or locked by MSBuild (e.g. PID file locks). By having a dedicated in-memory / workspace code engine, the LLM agent is given a real code problem to diagnose, fix, and verify without interfering with the host runner process.

### Decision 2: Real MAF `ChatClientAgent` Streaming Integration
- *Design*: Use `ChatClientAgent` with registered `AIFunctionFactory` tools (`InspectCode`, `ApplyCodeFix`, `CompileAndVerify`).
- *Rationale*: Demonstrates standard Microsoft Agent Framework v1.0+ patterns with real LLM reasoning tokens streamed live to terminal.

## Risks / Trade-offs

- [Risk] LLM might attempt to fix everything in a single turn without running verification.
  - *Mitigation*: Instruct the system prompt and tools to require incremental verification after each inspection/patch step, ensuring each loop cycle evaluates the live compiler state.
- [Risk] Missing LLM credentials.
  - *Mitigation*: Display clear security warning if credentials are not configured, while running the multi-stage loop cleanly with live LLM when configured.
