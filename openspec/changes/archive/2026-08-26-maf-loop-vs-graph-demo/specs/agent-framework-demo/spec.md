## Purpose

Demonstrate Loop Engineering vs Agentic Graph Engineering patterns using the official Microsoft Agent Framework (MAF) v1.0+ production API abstractions in a presentation-optimized console application.

## ADDED Requirements

### Requirement: Target framework and dependencies

The system SHALL target .NET 10.0 and reference official Microsoft Agent Framework packages.

#### Scenario: Project configuration

- **WHEN** the project is built
- **THEN** it targets `net10.0` and references `Microsoft.Agents.AI` and `Microsoft.Extensions.AI` packages

#### Scenario: Clean build

- **WHEN** `dotnet build` is executed
- **THEN** the build succeeds with zero warnings and zero errors

### Requirement: Program entry point

The system SHALL provide a main entry point that orchestrates demo execution.

#### Scenario: Demo initialization

- **WHEN** the application starts
- **THEN** `Program.cs` initializes a simulated Microsoft Foundry client context and runs both paradigms sequentially

#### Scenario: User selection

- **WHEN** the application starts
- **THEN** the user can choose to run Loop, Graph, or both demos

### Requirement: Loop paradigm demonstration

The system SHALL demonstrate an AIAgent executing an internal autonomous loop.

#### Scenario: Iterative correction loop

- **WHEN** the LoopAgentDemo runs
- **THEN** an `AIAgent` executes via `agent.RunAsync()` with a registered `CompileProject` tool that fails on first run and succeeds on second

#### Scenario: Loop visual output

- **WHEN** the loop executes
- **THEN** each iteration outputs iteration counter with `[Loop #X] [LLM REASONING]` (Blue), `[Loop #X] [TOOL CALL]` (Cyan), and `[Loop #X] [OBSERVATION]` (DarkGray) headers

#### Scenario: Loop border styling

- **WHEN** the loop section starts
- **THEN** it renders single-lined ASCII borders `[---]` in Yellow

### Requirement: Graph paradigm demonstration

The system SHALL demonstrate a Workflow graph with isolated nodes representing specialized coding micro-agents.

#### Scenario: DAG workflow execution

- **WHEN** the GraphWorkflowDemo runs
- **THEN** it executes a directed acyclic graph with ArchitectAgent and CoderAgent nodes

#### Scenario: Visual node routing

- **WHEN** the graph executes
- **THEN** it prints directional arrows `[ArchitectNode] ---> [CodingNode]` to visualize state routing

#### Scenario: Graph border styling

- **WHEN** the graph section starts
- **THEN** it renders thick, double-lined ASCII borders `[===]` in Magenta

#### Scenario: Parallel fan-out visualization

- **WHEN** parallel tasks execute
- **THEN** they render using structural tree branch lines `├──` and `└──` in DarkCyan

### Requirement: Governance middleware guardrail

The system SHALL demonstrate human-in-the-loop checkpoint via middleware interceptor.

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

### Requirement: Centralized console logging

The system SHALL use a ConsoleLogger class for all output formatting.

#### Scenario: No raw Console.WriteLine

- **WHEN** logic files are reviewed
- **THEN** they contain no raw `Console.WriteLine` statements (all routed through ConsoleLogger)

#### Scenario: Color-coded output

- **WHEN** logs are displayed
- **THEN** they use appropriate colors: Magenta for graph borders, Yellow for loop borders, Blue for LLM reasoning, Cyan for tool calls, DarkGray for observations, DarkRed for security warnings

#### Scenario: Timing delays

- **WHEN** major execution transformations occur
- **THEN** Thread.Sleep delays (800ms-1500ms) allow audience to watch orchestration unfold