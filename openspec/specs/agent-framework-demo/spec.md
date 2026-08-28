## Purpose

Demonstrate Loop Engineering vs Agentic Graph Engineering patterns using the official Microsoft Agent Framework (MAF) v1.0+ production API abstractions in a presentation-optimized console application.

## Requirements

### Requirement: Target framework and dependencies

The system SHALL target .NET 10.0, reference official Microsoft Agent Framework packages, and support seamless build and test automation.

#### Scenario: Project configuration

- **WHEN** the project is built
- **THEN** it targets `net10.0` and references `Microsoft.Agents.AI` and `Microsoft.Extensions.AI` packages

#### Scenario: Clean build

- **WHEN** `dotnet build` is executed
- **THEN** the build succeeds with zero warnings and zero errors

#### Scenario: Automated test execution

- **WHEN** `make test` or `dotnet test` is executed
- **THEN** the test runner accurately targets and executes the test project suite `tests/AksAgenticWorkflowConsole.Tests` with zero failures

### Requirement: Program entry point

The system SHALL provide a main entry point that orchestrates walkthrough execution.

#### Scenario: Demo initialization

- **WHEN** the application starts
- **THEN** `Program.cs` initializes a simulated Microsoft Foundry client context and runs both paradigms sequentially

#### Scenario: User selection

- **WHEN** the application starts
- **THEN** the user can choose to run Loop, Graph, Governance, or all walkthroughs

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

### Requirement: Governance middleware guardrail

The system SHALL demonstrate human-in-the-loop checkpoint via middleware interceptor in an interactive walkthrough.

#### Scenario: Deployment interception

- **WHEN** the graph attempts to hit the deployment node
- **THEN** the middleware intercepts the action and pauses the session

#### Scenario: Human approval prompt

- **WHEN** the middleware intercepts deployment
- **THEN** it displays a high-visibility warning block with DarkRed background and White text

#### Scenario: ASCII hand pointer

- **WHEN** the approval prompt is displayed
- **THEN** it shows an ASCII hand pointer `👉` next to the interactive prompt instruction

#### Scenario: Console authorization

- **WHEN** the user provides authorization
- **THEN** the workflow continues to the deployment node

#### Scenario: Real workflow middleware pipeline

- **WHEN** the workflow runs with guardrail middleware
- **THEN** the middleware intercepts the actual workflow context transition before executing the terminal node

### Requirement: Centralized console logging

The system SHALL use a ConsoleLogger class for all output formatting, brand presentation, and readable color contrast.

#### Scenario: Brand banner display

- **WHEN** the application starts
- **THEN** the system displays a prominent, large-font brand banner (`pronative.ai`) in a high-visibility theme color (`ConsoleColor.Cyan` or `ConsoleColor.Green`)

#### Scenario: No raw Console.WriteLine

- **WHEN** logic files are reviewed
- **THEN** they contain no raw `Console.WriteLine` statements (all routed through ConsoleLogger)

#### Scenario: Color-coded output

- **WHEN** logs are displayed
- **THEN** they use high-readability colors: Cyan/Green for brand banner & success, Magenta for graph borders, Yellow for loop borders, Blue for LLM reasoning, Cyan for tool calls, Gray for observations, and DarkRed for security warnings

#### Scenario: Timing delays

- **WHEN** major execution transformations occur
- **THEN** Thread.Sleep delays (800ms-1500ms) allow audience to watch orchestration unfold

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
