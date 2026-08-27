## Why

The current `LoopDiagnosticWorkspace.cs` returned static, hardcoded diagnostic strings rather than dynamically evaluating code via live LLM inference. To provide an authentic demonstration of autonomous agent looping, both code modification and code verification/feedback must be powered by real-time LLM calls using the Microsoft Agent Framework (MAF).

## What Changes

- Replace all hardcoded/canned diagnostic strings in `LoopDiagnosticWorkspace.cs` with dynamic, real-time LLM-powered code evaluation and verification.
- Implement an automated LLM Code Evaluator (`CodeVerifierAgent` / `CompilerAuditor`) in MAF that analyzes code submitted by the developer agent in real time, checks C# syntax, type safety, nullability, and quality criteria, and returns genuine dynamic compiler-style diagnostics with structured status (`[FAIL]`, `[WARNING]`, or `[PASS - VERIFIED]`).
- Enable an authentic autonomous multi-turn loop where:
  - `LoopDevAgent` streams reasoning and code patches.
  - Verification tool invokes real LLM evaluation on the modified code.
  - The developer agent adapts and refines its code based on live LLM evaluator feedback until clean convergence is confirmed.
- Remove all static mock diagnostic strings from the workspace.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agent-framework-demo`: Update the Loop paradigm specification so that code verification and diagnostic feedback in the autonomous loop are generated dynamically via real-time LLM evaluation rather than static simulated strings.

## Impact

- **`src/LoopParadigm/LoopDiagnosticWorkspace.cs`**: Refactored to accept an `IChatClient` (or evaluator agent) and perform dynamic, real-time code evaluation without hardcoded strings.
- **`src/LoopParadigm/LoopAgentWalkthrough.cs`**: Orchestrates live tool execution where the compilation/verification tool runs real-time LLM code evaluation.
- **Tests**: Update unit tests in `tests/AksAgenticWorkflowConsole.Tests/LoopDiagnosticTests.cs` to test the dynamic workspace and evaluator.
