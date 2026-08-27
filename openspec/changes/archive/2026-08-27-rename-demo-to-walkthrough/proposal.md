## Why

The project currently uses the term "Demo" across code, class names, console outputs, menus, and specifications. Changing the terminology from "Demo" to "Walkthrough" better reflects the educational, step-by-step nature of the Microsoft Agent Framework reference implementations and creates a more professional presentation experience.

## What Changes

- Rename demo orchestrator classes and files:
  - `LoopAgentDemo` -> `LoopAgentWalkthrough` (in `src/LoopParadigm/LoopAgentWalkthrough.cs`)
  - `GraphWorkflowDemo` -> `GraphWorkflowWalkthrough` (in `src/GraphParadigm/GraphWorkflowWalkthrough.cs`)
- Update interactive console menu, banners, headers, and log messages in `Program.cs`, `LoopAgentWalkthrough.cs`, `GraphWorkflowWalkthrough.cs`, and `MiddlewareGuardrail.cs` from "Demo" to "Walkthrough" (e.g. "Select a walkthrough to run", "LOOP ENGINEERING WALKTHROUGH", "GRAPH ENGINEERING WALKTHROUGH", "GOVERNANCE MIDDLEWARE WALKTHROUGH", "Run All Walkthroughs").
- Update test fixtures and mock goal names in `tests/AksAgenticWorkflowConsole.Tests/StateAndToolTests.cs`.
- Update inline comments and docstrings across `src/` to use "walkthrough" instead of "demo".
- Update capability requirements in `agent-framework-demo` delta spec to reflect the "walkthrough" terminology.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agent-framework-demo`: Update orchestration, loop, graph, and guardrail requirements to use "walkthrough" terminology instead of "demo".

## Impact

- **Code & API Surfaces**: Class renames `LoopAgentDemo` to `LoopAgentWalkthrough` and `GraphWorkflowDemo` to `GraphWorkflowWalkthrough`. Method `RunAllDemos()` in `Program.cs` becomes `RunAllWalkthroughs()`.
- **User Interface**: Console banners and selection menus display "Walkthrough" instead of "Demo".
- **Tests**: Test assertions referencing "demo" updated to "walkthrough".
