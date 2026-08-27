## MODIFIED Requirements

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
