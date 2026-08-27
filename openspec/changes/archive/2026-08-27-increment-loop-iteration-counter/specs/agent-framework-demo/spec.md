## MODIFIED Requirements

### Requirement: Loop paradigm demonstration

The system SHALL demonstrate an AIAgent executing an internal autonomous loop across multiple iterations (minimum 2 iterations, typically 3 to 4) with live tool execution, real-time LLM-driven diagnostic feedback, progressive iteration counter incrementation, warning refinement, and final clean convergence in an interactive walkthrough.

#### Scenario: Iterative correction loop

- **WHEN** the LoopAgentWalkthrough runs with an active LLM client
- **THEN** an `AIAgent` executes via `agent.RunStreamingAsync()` across multiple distinct loop cycles with registered live inspection, patch, and compilation verification tools until converging upon zero warnings and zero errors

#### Scenario: Live tool execution

- **WHEN** the agent calls the diagnostic or verification tools
- **THEN** the system executes real-time dynamic LLM evaluation against the current code buffer without hardcoded strings and returns live compiler/quality feedback back to the agent

#### Scenario: Loop visual output

- **WHEN** the loop executes
- **THEN** each iteration dynamically increments and outputs the iteration counter with `[Loop #X] [LLM REASONING]` (Blue), `[Loop #X] [TOOL CALL]` (Cyan), and `[Loop #X] [OBSERVATION]` (DarkGray) headers

#### Scenario: Loop border styling

- **WHEN** the loop section starts
- **THEN** it renders single-lined ASCII borders `[---]` in Yellow
