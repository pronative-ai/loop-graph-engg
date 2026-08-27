## Why

In the Loop Engineering Walkthrough, students need to clearly observe and understand the core concept of autonomous agent looping (Autonomous Iterative Correction). Currently, if the live build passes on the first run, the agent finishes in a single step without visibly looping through failure diagnosis, fix application, warning refinement, and final verification convergence. To make the educational experience impactful and realistic, the loop must execute a minimum of 2 to 4 iterations using real Microsoft Agent Framework (MAF) LLM calls and live tool interactions.

## What Changes

- Redesign `LoopAgentWalkthrough` to execute a multi-iteration autonomous loop (at least 3-4 iterations, minimum 2) powered by real LLM agent calls (`ChatClientAgent` with MAF):
  - **Iteration 1 (Initial Failure)**: Agent invokes verification tool; system returns initial compilation error (e.g., missing type/namespace or syntax bug). Agent reasons over diagnostics and generates a fix.
  - **Iteration 2 (Warning / Quality Defect)**: Agent applies fix and calls tool; system detects resolution of initial error but surfaces a secondary compiler warning (e.g., nullability warning or unhandled edge case). Agent reasons and formulates code refinement.
  - **Iteration 3 / 4 (Clean Convergence)**: Agent applies refined fix and runs verification; system confirms zero errors, zero warnings, and successful verification. Agent concludes the cycle with an engineering summary.
- Introduce realistic iterative diagnosis tools (e.g., `CompileAndVerifyTool`, `ApplyPatchTool`, `InspectDiagnosticsTool`) registered with `AIFunctionFactory` in MAF.
- Maintain real-time streaming output (`[Loop #X] [LLM REASONING]`, `[Loop #X] [TOOL CALL]`, `[Loop #X] [OBSERVATION]`) for each distinct loop cycle.
- Ensure 100% real LLM agent orchestration through Microsoft Agent Framework without synthetic skipping.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `agent-framework-demo`: Update the Loop paradigm requirements to mandate a multi-iteration autonomous correction cycle (minimum 2 iterations, typically 3-4) showing progressive error resolution, warning handling, and clean convergence.

## Impact

- **`src/LoopParadigm/LoopAgentWalkthrough.cs`**: Implemented with multi-stage iterative challenge context, real MAF `ChatClientAgent` streaming loop, and stateful verification tools.
- **Console Presentation**: Students and attendees clearly see multiple distinct loop iterations (`[Loop #1]`, `[Loop #2]`, `[Loop #3]`) unfolding live in terminal.
- **Tests**: Unit tests in `tests/AksAgenticWorkflowConsole.Tests` verify multi-iteration loop state and tool transitions.
