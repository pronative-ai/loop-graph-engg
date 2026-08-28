# AI Agentic Workflows: Loop vs Graph Engineering (MAF)

A .NET 10 console application demonstrating agentic workflow orchestration built on **Microsoft Agent Framework (MAF)**. The app hosts three interactive walkthroughs - Loop Engineering, Graph Engineering, and Governance Middleware - that contrast iterative self-correction against directed-acyclic graph (DAG) multi-agent routing, with human-in-the-loop guardrails and OpenTelemetry distributed tracing.

## Features

- **Loop Engineering Walkthrough**: Uses the official `Microsoft.Agents.AI.LoopAgent` with `LoopEvaluator` and `LoopAgentOptions` to drive an autonomous developer agent that iterates until build diagnostics report `[PASS - VERIFIED]`.
- **Graph Engineering Walkthrough**: Builds a multi-agent DAG (`Architect` -> parallel `Backend`/`Frontend` -> `Reviewer` -> `Deployment`) using MAF `WorkflowBuilder`, parallel fan-out/fan-in barriers, and conditional edges.
- **Governance Middleware Walkthrough**: Injects a human-authorization guardrail into the workflow pipeline, blocking the deployment transition until an operator approves.
- **Live LLM Tools**: Exposes `InspectCode`, `ApplyCodeFix`, and `CompileAndVerify` functions via `AIFunctionFactory`, equipped with OpenTelemetry tracing tags.
- **Deterministic Fallback**: Walkthroughs run in a simulated mode when no LLM gateway is configured.
- **OpenTelemetry (OTLP) Distributed Tracing**: Exports traces to any OTLP collector (Langfuse, SigNoz, Jaeger, .NET Aspire).
- **Subprocess Execution Tool**: `TerminalExecutionTool` runs shell commands (e.g. `dotnet build`) with captured stdout/stderr and exit codes.
- **Menu-Driven Console**: Select individual walkthroughs or run all sequentially.

## Prerequisites

- .NET 10 SDK (or later)
- Access to an OpenAI-compatible LLM gateway endpoint (e.g. `gateway.pronative.ai`) with an API key
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

Edit `.env` with your gateway URL, API key, and model:

```env
GATEWAY_URL=https://your-gateway-url.openai.azure.com/
GATEWAY_KEY=your-api-key-here
MODEL_NAME=gpt-4o
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

Once running, choose from the walkthrough menu:

1. **Loop Engineering** - iterative self-correction via official MAF `LoopAgent`
2. **Graph Engineering** - DAG multi-agent routing
3. **Governance Middleware** - human checkpoint guardrail
4. **Run All Walkthroughs** - execute all three sequentially
5. **Exit**

## Environment Variables

| Variable | Description | Required | Default |
|----------|-------------|----------|---------|
| `GATEWAY_URL` | OpenAI-compatible LLM gateway endpoint URL | Yes | - |
| `GATEWAY_KEY` | API key for gateway authentication | Yes | - |
| `MODEL_NAME` | Model to use on the gateway | No | `gpt-4o` |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OpenTelemetry OTLP collector endpoint | No | `https://dev-monitoring.pronative.ai/api/public/otel` |
| `OTEL_EXPORTER_OTLP_HEADERS` | Custom headers for the OTLP exporter (e.g. Langfuse Basic auth) | No | - |
| `OTEL_SERVICE_NAME` | Service name reported to the trace collector | No | `loop-vs-graph` |

Configuration is loaded from a `.env` file (gitignored) or the process environment via `DotNetEnv`.

## Architecture

The app is organized into three paradigms under `src/`, selected from the main menu (`src/Program.cs`).

### Loop Engineering (`LoopParadigm/`)

An autonomous developer agent wrapped by the official MAF `LoopAgent`. It cycles through live tools until the LLM verifier reports a clean build.

```
LoopAgent (MAF)
  │
  ├──→ InspectCode ─────────────────┐
  ├──→ ApplyCodeFix ────────────────┤  iterate until
  └──→ CompileAndVerify ────────────┘  STATUS: [PASS - VERIFIED]
```

### Graph Engineering (`GraphParadigm/`)

A directed acyclic graph of single-responsibility agents wired through the shared `AgenticWorkflow<TState>` engine (built on MAF `WorkflowBuilder` + `InProcessExecution`).

```
ArchitectNode
   │
   ├── [parallel split] ──► BackendCoderNode ──┐
   └──────────────────────► FrontendCoderNode ─┤
                                               ▼
                                      (parallel join) ReviewerNode
                                               │
                         ── conditional edge: state.IsApproved ──
                                               ▼
                                        DeploymentNode
```

### Governance Middleware (`Governance/`)

Demonstrates injecting a human-in-the-loop guardrail via `UseMiddleware`. Execution pauses at the transition to `DeploymentNode` and requires operator approval (or `deny`) before continuing.

```
PrepareReleaseNode ──► [MIDDLEWARE: Human Approval Checkpoint] ──► DeploymentNode
```

### Shared Components (`Shared/`)

- `ConsoleLogger` - centralized console formatting and presentation helpers
- `TelemetryConfiguration` - OpenTelemetry `TracerProvider` + OTLP exporter setup
- `WorkflowGraph.cs` - `AgenticWorkflow<TState>` engine: nodes, edges, parallel splits/joins, conditional edges, middleware pipeline
- `CodingProjectState` - blackboard-state container shared across all graph nodes
- `HumanCheckpointStore` - async approval/rejection tracker for human-in-the-loop gates
- `TerminalExecutionTool` - sandboxed subprocess execution and build verification
- `LlmConfiguration` - builds the MAF `IChatClient` from gateway environment variables

## Project Structure

```
LoopAgent/
├── LoopAgent.slnx
├── Makefile
├── .env.example
├── .gitignore
├── .aiignore
├── README.md
├── src/
│   ├── AksAgenticWorkflowConsole.csproj
│   ├── Program.cs
│   ├── GlobalUsings.cs
│   ├── LlmConfiguration.cs
│   ├── WorkflowGraph.cs
│   ├── CodingProjectState.cs
│   ├── HumanCheckpointStore.cs
│   ├── TerminalExecutionTool.cs
│   ├── Governance/
│   │   └── MiddlewareGuardrail.cs
│   ├── GraphParadigm/
│   │   └── GraphWorkflowWalkthrough.cs
│   ├── LoopParadigm/
│   │   ├── LoopAgentWalkthrough.cs
│   │   └── LoopDiagnosticWorkspace.cs
│   └── Shared/
│       ├── ConsoleLogger.cs
│       └── TelemetryConfiguration.cs
└── tests/
    └── AksAgenticWorkflowConsole.Tests/
        ├── AksAgenticWorkflowConsole.Tests.csproj
        ├── LlmConfigurationTests.cs
        ├── LoopDiagnosticTests.cs
        ├── StateAndToolTests.cs
        ├── TelemetryConfigurationTests.cs
        └── WorkflowGraphTests.cs
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

### Running Tests

The project uses xUnit (via `Microsoft.NET.Test.Sdk`). Run all tests with:

```bash
make test
```

or directly:

```bash
dotnet test tests/AksAgenticWorkflowConsole.Tests/AksAgenticWorkflowConsole.Tests.csproj
```

### Cleaning

```bash
make clean
```

## Configuration

The application reads configuration from environment variables (or a `.env` file, which is gitignored for security). See `.env.example` for all available options.

## Security Notes

- Never commit `.env` files containing real credentials
- Use a secrets manager (e.g. Azure Key Vault) for production secrets
- The `.gitignore` and `.aiignore` exclude `.env` files by default

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests (`make test`)
5. Submit a pull request

## Support

For issues and questions:
- Check the repository issues page
- Review the [Microsoft Agent Framework](https://learn.microsoft.com/en-us/agent-framework/) documentation
- See the [MAF LoopAgent documentation](https://learn.microsoft.com/en-us/agent-framework/agents/looping?pivots=programming-language-csharp)