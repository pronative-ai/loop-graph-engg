## Purpose

Orchestrate multi-agent workflows using Microsoft Agent Framework (MAF) with agentic graph primitives including nodes, edges, parallel execution, and conditional routing.

## Requirements

### Requirement: Workflow state management

The system SHALL maintain a `CodingProjectState` object that tracks workflow progress, tasks, agent outputs, and approval status.

#### Scenario: Initialize workflow state

- **WHEN** a new workflow is created with a goal description
- **THEN** the system creates a `CodingProjectState` with the goal set and all status flags initialized to false

#### Scenario: Track task creation

- **WHEN** the Planner node executes
- **THEN** the system sets `TasksCreated` to true in the workflow state

#### Scenario: Track approval status

- **WHEN** the Reviewer node completes evaluation
- **THEN** the system sets `IsApproved` to true or false based on code quality checks

#### Scenario: Store intermediate agent outputs
- **WHEN** each agent node completes execution
- **THEN** the generated code, architecture, or review is stored in `CodingProjectState` for subsequent nodes to consume

### Requirement: Agent node definition

The system SHALL support defining AI agent nodes with custom instructions, names, and registered tools.

#### Scenario: Create backend agent

- **WHEN** a BackendCoder agent node is defined
- **THEN** the system creates an agent with backend C# development instructions and a terminal execution tool

#### Scenario: Create frontend agent

- **WHEN** a FrontendCoder agent node is defined
- **THEN** the system creates an agent with frontend Blazor development instructions

#### Scenario: Register tools on agent

- **WHEN** an agent has tools registered
- **THEN** the agent can execute tool calls during its workflow execution

### Requirement: Agentic graph orchestration

The system SHALL orchestrate workflow execution using official Microsoft Agent Framework `Microsoft.Agents.AI.Workflows.WorkflowBuilder` and `Workflow` primitives with native executors, node routing, and transition events.

#### Scenario: Linear node execution

- **WHEN** a workflow has sequential nodes connected by edges
- **THEN** the system executes nodes using MAF `Workflow` in order, passing state or message payloads between them

#### Scenario: Parallel split execution

- **WHEN** the Architect/Planner node completes and triggers a parallel split
- **THEN** the system executes BackendCoder and FrontendCoder nodes concurrently using MAF's native workflow fan-out / concurrent executor capabilities

#### Scenario: Parallel join synchronization

- **WHEN** both parallel nodes complete
- **THEN** the system synchronizes concurrent outputs using MAF's native workflow join / aggregation mechanism and proceeds to the Reviewer node

#### Scenario: Conditional edge routing

- **WHEN** the Reviewer node completes and `IsApproved` is true
- **THEN** the system routes to the Deployment node using MAF conditional edges or router switches

#### Scenario: Terminal node completion

- **WHEN** a terminal node is reached
- **THEN** the MAF workflow execution completes successfully