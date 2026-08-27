## MODIFIED Requirements

### Requirement: Loop paradigm demonstration

The system SHALL demonstrate an AIAgent executing an internal autonomous loop across multiple iterations (minimum 2 iterations, typically 3 to 4) with live tool execution, mandatory real-time LLM-driven diagnostic feedback, progressive iteration counter incrementation, warning refinement, and final clean convergence in an interactive walkthrough, recording all tool inputs and outputs.

#### Scenario: Iterative correction loop

- **WHEN** the LoopAgentWalkthrough runs with an active LLM client
- **THEN** an `AIAgent` executes via `agent.RunStreamingAsync()` across multiple distinct loop cycles with registered live inspection, patch, and compilation verification tools until converging upon zero warnings and zero errors

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

The system SHALL demonstrate a Workflow graph executing an end-to-end directed acyclic graph with isolated nodes representing specialized coding micro-agents in an interactive walkthrough, capturing incoming specifications and generated artifacts on node execution spans.

#### Scenario: DAG workflow execution

- **WHEN** the GraphWorkflowWalkthrough runs with an active LLM client
- **THEN** it executes an `AgenticWorkflow<CodingProjectState>` directed acyclic graph passing state between ArchitectAgent, CoderAgent, and ReviewerAgent nodes

#### Scenario: Graph node payload logging

- **WHEN** a workflow DAG node executes
- **THEN** the system attaches incoming state properties (e.g. `ArchitecturalSpec`, `Goal`) as input tags and records outgoing state results (e.g. `BackendCode`, `FrontendCode`, `ReviewFeedback`, `DeploymentLogs`) on the node activity span

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

### Requirement: OpenTelemetry observability

The system SHALL initialize OpenTelemetry distributed tracing and export all agent runs, LLM requests, tool invocations, and workflow transitions with full input and output payloads to the configured OpenTelemetry (OTLP) collector endpoint.

#### Scenario: OpenTelemetry tracer initialization

- **WHEN** the application starts
- **THEN** the system configures an OpenTelemetry `TracerProvider` subscribing to Microsoft Agent Framework (`Microsoft.Agents.AI.*`), Microsoft Extensions AI (`Microsoft.Extensions.AI.*`), and application activity sources

#### Scenario: ChatClient OpenTelemetry instrumentation

- **WHEN** the `IChatClient` is constructed
- **THEN** the client is wrapped with `.AsBuilder().UseOpenTelemetry()` so every LLM turn, model request, token metric, and completion automatically generates OpenTelemetry spans

#### Scenario: Agent prompt and response payload logging

- **WHEN** agents execute prompts and receive streaming or non-streaming responses
- **THEN** input prompts, agent roles, and generated text outputs are attached as telemetry span attributes and displayed in console stream output

#### Scenario: OTLP trace export

- **WHEN** traces and spans are recorded during agent execution
- **THEN** the system securely exports spans via OTLP to the configured endpoint (e.g. `https://dev-monitoring.pronative.ai/api/public/otel` or SigNoz) with required ingestion headers

#### Scenario: Trace lifecycle management

- **WHEN** the application shuts down or completes a walkthrough
- **THEN** all pending OpenTelemetry trace spans are flushed immediately before moving to the next interaction or process exit
