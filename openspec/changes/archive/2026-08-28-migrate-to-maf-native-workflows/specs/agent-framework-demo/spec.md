## MODIFIED Requirements

### Requirement: Loop paradigm demonstration

The system SHALL demonstrate an autonomous agent executing iterative refinement using Microsoft Agent Framework's official `Microsoft.Agents.AI.LoopAgent` wrapped around a base agent with `LoopEvaluator` and `LoopAgentOptions`, executing live tools and real-time compiler diagnostics until completion criteria are satisfied.

#### Scenario: Iterative correction loop

- **WHEN** the LoopAgentWalkthrough runs with an active LLM client
- **THEN** it executes an official `Microsoft.Agents.AI.LoopAgent` wrapped with `CompletionMarkerLoopEvaluator` or `DelegateLoopEvaluator` across multiple distinct cycles with registered live inspection, patch, and compilation verification tools until converging upon zero warnings and zero errors

#### Scenario: Live tool execution

- **WHEN** the agent calls the diagnostic or verification tools
- **THEN** the system executes real-time dynamic LLM evaluation against the current code buffer with a mandatory `IChatClient` (disallowing offline fallback strings) and returns live compiler/quality feedback back to the agent

#### Scenario: Tool input and output logging

- **WHEN** any tool (`InspectCode`, `ApplyCodeFix`, `CompileAndVerify`) is invoked during loop iterations
- **THEN** the system records all input parameters (e.g. file paths, patches, build targets) and output results (e.g. code content, compiler outputs, diagnostics) directly on the tool's activity span attributes and structured console logs

#### Scenario: Loop visual output

- **WHEN** the loop executes
- **THEN** each iteration dynamically increments and outputs the iteration counter with `[Loop #X] [LLM REASONING]` (Blue), `[Loop #X] [TOOL CALL]` (Cyan), and `[Loop #X] [OBSERVATION]` (DarkGray) headers

#### Scenario: Loop border styling

- **WHEN** the loop section starts
- **THEN** it renders single-lined ASCII borders `[---]` in Yellow

### Requirement: Graph paradigm demonstration

The system SHALL demonstrate a Workflow graph executing an end-to-end directed acyclic graph built with Microsoft Agent Framework's `Microsoft.Agents.AI.Workflows.WorkflowBuilder` and `Workflow` primitives with specialized coding micro-agents in an interactive walkthrough.

#### Scenario: DAG workflow execution

- **WHEN** the GraphWorkflowWalkthrough runs with an active LLM client
- **THEN** it executes a MAF `Workflow` built via `WorkflowBuilder` passing state between ArchitectAgent, CoderAgent, and ReviewerAgent nodes

#### Scenario: Graph node payload logging

- **WHEN** a workflow DAG node executes
- **THEN** the system attaches incoming state properties (e.g. `ArchitectureSpec`, `Goal`) as input tags and records outgoing state results (e.g. `BackendCode`, `FrontendCode`, `ReviewNotes`, `DeploymentLogs`) on the node activity span

#### Scenario: Visual node routing

- **WHEN** the graph executes
- **THEN** it prints directional arrows `[ArchitectNode] ---> [CodingNode]` to visualize state routing

#### Scenario: Graph border styling

- **WHEN** the graph section starts
- **THEN** it renders thick, double-lined ASCII borders `[===]` in Magenta

#### Scenario: Parallel fan-out visualization

- **WHEN** parallel tasks execute
- **THEN** they render using structural tree branch lines `├──` and `└──` in DarkCyan

#### Scenario: State propagation across nodes

- **WHEN** an upstream agent finishes producing architectural specifications
- **THEN** downstream coder nodes receive the generated specifications directly in their execution context
