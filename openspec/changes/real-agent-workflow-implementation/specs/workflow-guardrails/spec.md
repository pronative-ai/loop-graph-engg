## MODIFIED Requirements

### Requirement: Deployment checkpoint verification

The system SHALL require human approval through the middleware pipeline before proceeding to deployment operations.

#### Scenario: Block deployment without approval

- **WHEN** the workflow attempts to transition to the Deployment node
- **THEN** the system pauses execution and triggers an approval prompt

#### Scenario: Approval granted

- **WHEN** a human approves the deployment checkpoint
- **THEN** the system continues execution to the Deployment node

#### Scenario: Approval rejected

- **WHEN** a human rejects the deployment checkpoint
- **THEN** the system throws an `UnauthorizedAccessException` with rejection message

#### Scenario: Real-time operator interaction

- **WHEN** the checkpoint triggers in console mode
- **THEN** the operator is prompted interactively to authorize or deny the deployment before any deployment action runs
