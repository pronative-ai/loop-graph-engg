## MODIFIED Requirements

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
