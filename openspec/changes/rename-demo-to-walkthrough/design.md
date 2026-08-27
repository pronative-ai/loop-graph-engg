## Context

The codebase contains several classes, methods, console strings, and comments using "Demo" (e.g. `LoopAgentDemo`, `GraphWorkflowDemo`, `RunAllDemos`, "LOOP ENGINEERING DEMO", etc.). See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Systematically rename orchestrator classes (`LoopAgentDemo` -> `LoopAgentWalkthrough`, `GraphWorkflowDemo` -> `GraphWorkflowWalkthrough`) and their filenames.
- Update `Program.cs` menu items, banners, and dispatcher methods (`RunAllDemos` -> `RunAllWalkthroughs`).
- Update console banners, log messages, and headers across `src/Governance/MiddlewareGuardrail.cs`, `src/LoopParadigm/`, `src/GraphParadigm/`.
- Update test cases in `tests/AksAgenticWorkflowConsole.Tests/` to use "walkthrough" references.
- Update code comments across `src/` to consistently refer to "walkthrough".

**Non-Goals:**
- Changing runtime behavior or execution flows of the Loop, Graph, or Governance agents.
- Renaming the project/solution files or repository names unless requested.

## Decisions

### Decision 1: Rename files and classes consistently
- Rename `src/LoopParadigm/LoopAgentDemo.cs` -> `src/LoopParadigm/LoopAgentWalkthrough.cs` and class `LoopAgentWalkthrough`.
- Rename `src/GraphParadigm/GraphWorkflowDemo.cs` -> `src/GraphParadigm/GraphWorkflowWalkthrough.cs` and class `GraphWorkflowWalkthrough`.
- *Rationale*: Keeps file names 1:1 aligned with C# class names as per .NET conventions.

### Decision 2: Update console UI strings
- In `Program.cs`:
  - Header: `=== Microsoft Agent Framework Walkthrough ===`
  - Menu prompt: `Select a walkthrough to run:`
  - Options: `1. Loop Engineering Walkthrough`, `2. Graph Engineering Walkthrough`, `3. Governance Middleware Walkthrough`, `4. Run All Walkthroughs`
  - Method: `RunAllWalkthroughs()`
- In `LoopAgentWalkthrough.cs`: `LOOP ENGINEERING WALKTHROUGH`
- In `GraphWorkflowWalkthrough.cs`: `GRAPH ENGINEERING WALKTHROUGH`
- In `MiddlewareGuardrail.cs`: `GOVERNANCE MIDDLEWARE WALKTHROUGH`

## Risks / Trade-offs

- [Risk] Broken references across project files or tests after renaming classes.
  - *Mitigation*: Run `dotnet build` and `dotnet test` to guarantee clean compilation and test success with zero errors.
