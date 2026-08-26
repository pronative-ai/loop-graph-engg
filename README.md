# AKS Agentic Workflow Console

A .NET 10 console application demonstrating agentic workflow orchestration using Microsoft Agent Framework (MAF). This application implements a multi-agent coding workflow with backend and frontend AI agents, orchestrated through an agentic graph with parallel execution, conditional routing, and middleware guardrails.

## Features

- **Multi-Agent Orchestration**: Backend and Frontend AI agents working in parallel
- **Agentic Graph Engine**: Directed workflow with nodes, edges, and conditional routing
- **Middleware Guardrails**: Human checkpoint verification before deployment
- **AKS Agent Gateway Integration**: Configurable LLM endpoint via environment variables
- **Parallel Execution**: Concurrent agent operations with synchronization

## Prerequisites

- .NET 10 SDK (or later)
- Access to an AKS agent gateway endpoint
- Environment variables configured (see below)

## Quick Start

### 1. Clone and Setup

```bash
git clone <repository-url>
cd LoopAgent
```

### 2. Configure Environment

Copy the example environment file and fill in your values:

```bash
cp .env.example .env
```

Edit `.env` with your AKS agent gateway URL and key:

```env
AKS_AGENT_GATEWAY_URL=https://your-aks-gateway-url.openai.azure.com/
AKS_AGENT_GATEWAY_KEY=your-api-key-here
```

### 3. Build and Run

Using Make:

```bash
# Build the project
make build

# Run the application
make run
```

Or using dotnet CLI:

```bash
# Build
dotnet build src/AksAgenticWorkflowConsole.csproj

# Run
dotnet run --project src/AksAgenticWorkflowConsole.csproj
```

## Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `AKS_AGENT_GATEWAY_URL` | Your AKS agent gateway endpoint URL | Yes |
| `AKS_AGENT_GATEWAY_KEY` | API key for authentication | Yes |

## Architecture

The application uses Microsoft Agent Framework (MAF) to orchestrate a workflow graph:

```
Planner → [BackendCoder, FrontendCoder] → Reviewer → Deployment
```

- **Planner**: Breaks down tasks for parallel execution
- **BackendCoder**: Handles backend C# development tasks
- **FrontendCoder**: Handles frontend Blazor development tasks
- **Reviewer**: Evaluates code quality and approval
- **Deployment**: Final deployment with human checkpoint verification

## Project Structure

```
LoopAgent/
├── src/
│   ├── AksAgenticWorkflowConsole.csproj
│   ├── Program.cs
│   ├── CodingProjectState.cs
│   ├── TerminalExecutionTool.cs
│   ├── HumanCheckpointStore.cs
│   └── WorkflowGuardrails.cs
├── .env.example
├── .gitignore
├── .aiignore
├── Makefile
└── README.md
```

## Make Targets

| Target | Description |
|--------|-------------|
| `make build` | Build in Release mode |
| `make build-debug` | Build in Debug mode |
| `make run` | Run the application |
| `make run-release` | Run in Release mode |
| `make restore` | Restore NuGet packages |
| `make clean` | Clean build artifacts |
| `make test` | Run tests |
| `make format` | Format code |
| `make publish` | Publish for deployment |
| `make help` | Show available targets |

## Development

### Building

```bash
make build
```

### Running

```bash
make run
```

### Cleaning

```bash
make clean
```

## Configuration

The application reads configuration from environment variables. For development, you can use a `.env` file (which is gitignored for security).

See `.env.example` for all available configuration options.

## Security Notes

- Never commit `.env` files containing real credentials
- Use Azure Key Vault or similar for production secrets
- The `.gitignore` excludes `.env` files by default

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests (`make test`)
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
- Check the [Issues](../../issues) page
- Review the Microsoft Agent Framework documentation
