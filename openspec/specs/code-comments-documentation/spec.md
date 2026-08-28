# code-comments-documentation Specification

## Purpose

Defines a consistent convention for inline code comments that improve developer readability during self-review, and a HIGHLIGHT marker that flags the most presenter-relevant code areas for a live walkthrough.

## Requirements

### Requirement: Developer-facing comments
The system SHALL include concise comments in source files that explain the purpose of core types, key methods, and non-obvious control flow without restating the code itself.

#### Scenario: Core types are commented
- **WHEN** a public class, record, or interface is defined in src/
- **THEN** it includes a comment (or XML doc summary) explaining its responsibility in the workflow

#### Scenario: Non-obvious control flow is commented
- **WHEN** a method contains conditional routing, parallel execution, or iteration that is not immediately obvious
- **THEN** a comment explains the intent of that control flow

#### Scenario: Comments avoid redundancy
- **WHEN** a comment is added next to a statement
- **THEN** it explains why or what the code does at a higher level rather than restating the code verbatim

### Requirement: HIGHLIGHT marker for presentation
The system SHALL use a HIGHLIGHT marker as an additional tag/comment on the key code areas intended for the live presentation, so the presenter can quickly navigate to them.

#### Scenario: Key areas are marked
- **WHEN** a code section is identified as a key presentation area
- **THEN** it is tagged with a single-line comment containing the HIGHLIGHT marker accompanied by a short note of what to highlight

#### Scenario: Marker is discoverable
- **WHEN** a reviewer searches the source for the string HIGHLIGHT
- **THEN** every presenter-relevant area is listed, ordered so the presentation path can be followed top-to-bottom

#### Scenario: Marker does not alter behavior
- **WHEN** a file containing HIGHLIGHT markers is built and run
- **THEN** output and behavior are identical to the same file without the markers

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

### Requirement: Comment style consistency
The system SHALL follow a consistent comment style that matches the project's C# conventions and is compatible with self-service review.

#### Scenario: Style is uniform
- **WHEN** multiple files in src/ are reviewed
- **THEN** comments use the same tense and structure so the codebase reads consistently

#### Scenario: Tests remain valid
- **WHEN** tests are run after comments are applied
- **THEN** all existing tests pass without modification
