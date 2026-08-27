## Why

The presence of "Offline build check" fallbacks in `LoopDiagnosticWorkspace.cs` and test method naming `CompileAndVerifyAsync_Offline_IncrementsIteration` introduced ambiguity about whether live LLM execution is strictly required. To ensure zero hardcoded responses and enforce that all code verification is genuinely executed by live `IChatClient` models in Microsoft Agent Framework (MAF), `CompileAndVerifyAsync` must mandate a non-null `IChatClient`, and test suites must validate real `IChatClient` interaction.

## What Changes

- Modify `LoopDiagnosticWorkspace.CompileAndVerifyAsync(IChatClient chatClient)` to strictly require a valid `IChatClient` parameter (throwing `ArgumentNullException` if null), eliminating all "Offline build check" fallback strings.
- Refactor unit tests in `LoopDiagnosticTests.cs`:
  - Rename `CompileAndVerifyAsync_Offline_IncrementsIteration` to `CompileAndVerifyAsync_WithChatClient_ExecutesEvaluationAndTracksState`.
  - Provide an `IChatClient` instance to test realistic prompt evaluation and state convergence without offline strings.
- Ensure the entire Loop Engineering Walkthrough exclusively operates on live LLM inference.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agent-framework-demo`: Enforce that the Loop paradigm verification tool strictly requires a live `IChatClient` and operates with zero hardcoded/offline string fallbacks.

## Impact

- **`src/LoopParadigm/LoopDiagnosticWorkspace.cs`**: `CompileAndVerifyAsync` parameter `IChatClient chatClient` is mandatory.
- **`tests/AksAgenticWorkflowConsole.Tests/LoopDiagnosticTests.cs`**: Uses an in-memory/mock `IChatClient` to test genuine evaluation flows without offline fallbacks.
