## Why

Create a .NET 10 console application demonstrating agentic workflow orchestration using Microsoft Agent Framework (MAF). The application will implement a multi-agent coding workflow with backend and frontend AI agents, orchestrated through an agentic graph with parallel execution, conditional routing, and middleware guardrails. The LLM endpoint will point to an AKS agent gateway URL instead of Azure Foundry, with environment variable configuration for deployment flexibility.

## What Changes

- New .NET 10 C# 14 console application project structure
- Implementation of `CodingProjectState` workflow state class
- `TerminalExecutionTool` for terminal command execution
- `HumanCheckpointStore` middleware for deployment verification
- Agentic workflow graph with Planner → Backend/Frontend parallel split → Reviewer → Deployment flow
- Configuration via `.env` and `.env.example` files for AKS agent gateway URL and key
- Project infrastructure: `.gitignore`, `.aiignore`, `Makefile`, `README.md`

## Capabilities

### New Capabilities

- `agentic-workflow-core`: Core workflow orchestration with agentic graph engine, node definitions, edges, and parallel execution
- `aks-gateway-integration`: LLM client configuration pointing to AKS agent gateway URL with environment variable support
- `workflow-guardrails`: Middleware guardrails for deployment verification with human checkpoint approval

### Modified Capabilities

None - this is a new project.

## Impact

- New .NET 10 console application project in repository root
- Dependencies: `Azure.Identity`, `Microsoft.Agents.AI`, `Microsoft.Agents.AI.Workflows`
- Environment variables: `AKS_AGENT_GATEWAY_URL`, `AKS_AGENT_GATEWAY_KEY`
- Build system: `Makefile` for build, run, clean commands
- Documentation: `README.md` with setup instructions
