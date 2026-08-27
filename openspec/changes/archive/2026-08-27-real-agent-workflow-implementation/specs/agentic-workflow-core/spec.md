## MODIFIED Requirements

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

### Requirement: Agentic graph orchestration

The system SHALL orchestrate workflow execution using a directed graph with nodes, edges, middleware pipelines, and transition rules.

#### Scenario: Linear node execution

- **WHEN** a workflow has sequential nodes connected by edges
- **THEN** the system executes nodes in order, passing state between them

#### Scenario: Parallel split execution

- **WHEN** the Planner node completes and triggers a parallel split
- **THEN** the system executes BackendCoder and FrontendCoder nodes concurrently

#### Scenario: Parallel join synchronization

- **WHEN** both parallel nodes complete
- **THEN** the system joins execution and proceeds to the Reviewer node

#### Scenario: Conditional edge routing

- **WHEN** the Reviewer node completes and `IsApproved` is true
- **THEN** the system routes to the Deployment node

#### Scenario: Terminal node completion

- **WHEN** a terminal node is reached
- **THEN** the workflow execution completes successfully
