## ADDED Requirements

### Requirement: Architectural flow commentary for agent paradigms
The system SHALL provide explanatory architectural flow comments across core execution files distinguishing Autonomous Loop (single-agent tool evaluation loop) from Graph Workflow (multi-agent DAG routing with human checkpointing and governance).

#### Scenario: Loop execution flow is commented
- **WHEN** reviewing `src/LoopParadigm/LoopAgentWalkthrough.cs` or `src/LoopParadigm/LoopDiagnosticWorkspace.cs`
- **THEN** comments clearly delineate agent definition, tool binding, autonomous iteration cycle, and termination conditions

#### Scenario: Graph workflow execution flow is commented
- **WHEN** reviewing `src/GraphParadigm/GraphWorkflowWalkthrough.cs` or `src/WorkflowGraph.cs`
- **THEN** comments explain state initialization, multi-agent node registration, conditional edge transitions, and checkpoint serialization

#### Scenario: Governance and middleware flow is commented
- **WHEN** reviewing `src/Governance/MiddlewareGuardrail.cs`
- **THEN** comments explain how function invocations are intercepted, validated, and logged before tool execution

### Requirement: Standardized HIGHLIGHT walkthrough markers
The system SHALL place standardized `// HIGHLIGHT: <Topic> - <Presenter Guidance>` comments on key walkthrough touchpoints to allow immediate navigation during live presentations and code demos.

#### Scenario: Key presentation anchors are searchable
- **WHEN** searching for `// HIGHLIGHT:` across the codebase
- **THEN** anchors appear at critical decision points: Program entry dispatch, ChatClientAgent instantiation, tool invocation loop, DAG node execution, human-in-the-loop checkpointing, and governance middleware evaluation

#### Scenario: Guidance clarifies the demo focus
- **WHEN** a developer views a `// HIGHLIGHT:` comment
- **THEN** the comment succinctly states what capability or concept the presenter should explain at that location
