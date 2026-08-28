## Context

The LoopAgent project illustrates two primary AI agent orchestration paradigms using Microsoft Agent Framework (MAF) in .NET 10 / C# 14:
1. **Loop Paradigm (Autonomous Iterative Agent)**: Single `ChatClientAgent` armed with terminal execution tools running inside a feedback loop until goal completion.
2. **Graph Paradigm (Multi-Agent DAG Workflow)**: Deterministic state machine with multiple specialized agents (Architect, Coder, Reviewer, Runner), human-in-the-loop checkpoints, and governance middleware guardrails.

During live demonstrations, presenters need a guided tour through the source files. Adding structured flow comments and standardized `// HIGHLIGHT:` anchors enables presenters and reviewing developers to quickly grasp the agent mechanics without cognitive overload.

## Goals / Non-Goals

**Goals:**
- Define a uniform tagging schema: `// HIGHLIGHT: <Concept> - <Talking point / explanation>`.
- Add clear architectural block comments explaining state transitions, agent handoffs, function calling mechanics, and governance validation.
- Cover all key files across entry points (`Program.cs`), loop paradigm (`LoopAgentWalkthrough.cs`, `LoopDiagnosticWorkspace.cs`), graph paradigm (`GraphWorkflowWalkthrough.cs`, `WorkflowGraph.cs`), governance (`MiddlewareGuardrail.cs`), checkpointing (`HumanCheckpointStore.cs`), and execution tools (`TerminalExecutionTool.cs`).

**Non-Goals:**
- Modifying runtime logic, execution flow, or agent prompt templates.
- Changing method signatures, public interfaces, or unit test definitions.
- Over-commenting boilerplate or obvious standard C# constructs.

## Decisions

### Decision 1: HIGHLIGHT Comment Prefix and Structure
- **Format**: `// HIGHLIGHT: [Topic] - [Presenter/Developer Takeaway]`
- **Rationale**: Searching for `// HIGHLIGHT:` or `HIGHLIGHT` in an IDE allows a presenter to step through the entire demo script chronologically or jump to specific topics (e.g. `HIGHLIGHT: Autonomous Loop Tool Call`, `HIGHLIGHT: DAG State Transition`, `HIGHLIGHT: Governance Interception`).
- **Alternative Considered**: Custom attributes or XML tags. Rejected because inline comments are lighter, language-native, and zero-overhead.

### Decision 2: Stage & Lifecycle Header Comments
- **Format**: Multi-line block comments (`/* --- STAGE X: [Title] --- */`) or structured section banners before key methods or logical phases in walkthrough classes.
- **Rationale**: Visually sections long walkthrough scripts into distinct stages matching the console demo outputs.

## Risks / Trade-offs

- **[Risk]** Code clutter from excessive comments.
  - **Mitigation**: Focus on high-signal architectural explanations, decision points, and presenter anchors. Avoid echoing obvious line-by-line code.
- **[Risk]** Divergence between comments and evolving code logic.
  - **Mitigation**: Tie comments directly to the enduring architectural principles (MAF ChatClientAgent loop, DAG node transitions, middleware pipelines) rather than transient variable names.
