## Context

The initial prototype in `LoopDiagnosticWorkspace.cs` used a static state switch returning predetermined strings. To provide a genuine demonstration of Loop Engineering for students, code verification must be performed through real-time LLM evaluation on the developer agent's actual code modifications. See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Eliminate all hardcoded canned diagnostic strings from `LoopDiagnosticWorkspace.cs`.
- Implement dynamic real-time LLM code evaluation in `LoopDiagnosticWorkspace.CompileAndVerifyAsync(IChatClient chatClient)`.
- Use a dedicated Roslyn/Compiler auditor prompt that evaluates the developer agent's C# code in real-time, producing authentic compiler-style error diagnostics, warnings, and pass/fail statuses (`[FAIL]`, `[WARNING]`, `[PASS - VERIFIED]`).
- Enable full dynamic collaboration between `LoopDevAgent` (authoring/patching code) and the Compiler Evaluator (validating code in real-time).
- Allow the loop to naturally cycle through failure, warning refinement, and convergence based on live LLM assessments.

**Non-Goals:**
- Introducing external process dependencies that require locking the host application binary.

## Decisions

### Decision 1: Live LLM Compiler & Quality Evaluator
- *Design*: Instead of static strings, `LoopDiagnosticWorkspace` uses `IChatClient` to execute a structured compiler/quality evaluation on the active source code buffer.
- *Rationale*: Guarantees 100% real-time LLM validation. The developer agent's specific patches are genuinely analyzed and critiqued on every loop iteration.

### Decision 2: Real-time Dynamic Convergence Detection
- *Design*: When the LLM evaluator confirms zero errors, zero warnings, and clean quality verification (tagged with `STATUS: [PASS - VERIFIED]`), `IsClean` is set to `true`, allowing `LoopAgentWalkthrough` to converge naturally.

## Risks / Trade-offs

- [Risk] Evaluator LLM formatting variance.
  - *Mitigation*: Provide crisp, structured prompt instructions to the evaluator to output standardized compiler diagnostic headers and clear `[FAIL]`, `[WARNING]`, `[PASS - VERIFIED]` status lines.
