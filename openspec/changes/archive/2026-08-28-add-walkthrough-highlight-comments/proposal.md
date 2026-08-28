## Why

Developers and presenters navigating the LoopAgent codebase need immediate visual cues and high-level architectural narratives embedded directly in the source code. Adding structured flow comments and standardized `HIGHLIGHT:` markers will enable rapid onboarding, effortless live demos, and clear code walkthroughs explaining both the Loop (autonomous iterative agent) and Graph (multi-agent DAG routing with human-in-the-loop & governance middleware) execution paradigms.

## What Changes

- Add structured block comments and inline flow descriptions explaining execution lifecycles, state transitions, and tool-calling loops.
- Embed standardized `// HIGHLIGHT: <topic>` markers across key walkthrough touchpoints (Program entry point, Autonomous Loop agent, DAG Graph Workflow, Governance Middleware, Checkpoint Storage, and Terminal Tool execution).
- Ensure comments provide clear conceptual context ("why" and "how the agent reasons/routes") without redundant code restatement.
- Maintain existing functional behavior, compilation integrity, and test pass status.

## Capabilities

### New Capabilities
<!-- None -->

### Modified Capabilities
- `code-comments-documentation`: Refine and clarify the HIGHLIGHT marker guidelines and flow explanation requirements across the console walkthrough files, governance layers, and execution tooling.

## Impact

- **Affected Source Files**:
  - `src/Program.cs`
  - `src/LoopParadigm/LoopAgentWalkthrough.cs`
  - `src/LoopParadigm/LoopDiagnosticWorkspace.cs`
  - `src/GraphParadigm/GraphWorkflowWalkthrough.cs`
  - `src/WorkflowGraph.cs`
  - `src/Governance/MiddlewareGuardrail.cs`
  - `src/HumanCheckpointStore.cs`
  - `src/TerminalExecutionTool.cs`
  - `src/CodingProjectState.cs`
  - `src/LlmConfiguration.cs`
- **APIs & Runtime**: No breaking changes, no API signature modifications, zero functional disruption to agent execution or tests.
