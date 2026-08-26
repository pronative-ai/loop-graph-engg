# Proposal: Agent Framework Loop vs Graph Presentation Demo

## Summary

Build a complete, runnable C# .NET console application that demonstrates Loop Engineering vs Agentic Graph Engineering patterns using the official Microsoft Agent Framework (MAF) v1.0+ production API abstractions. Optimized for on-stage presentation with extensive visual logging.

## Motivation

- **Showcase MAF capabilities**: Demonstrate the official Microsoft Agent Framework abstractions (`IChatClient`, `AIAgent`, `Workflow`, Middleware) in a polished, runnable demo.
- **Compare paradigms**: Visually contrast Loop Engineering (iterative autonomous correction) vs Graph Engineering (DAG state routing) patterns.
- **Governance demonstration**: Show human-in-the-loop checkpoint via middleware interceptor.

## Scope

- Existing .NET 10.0 console application (extend `src/` folder)
- Four demo modules: Loop, Graph, Governance, Shared Logger
- Extensive visual logging with color-coded output
- Mock LLM responses (no real API calls required)
- Compiles and runs with `dotnet run`

## Out of Scope

- Real LLM API integration (mock responses for demo)
- Unit tests (presentation demo)
- CI/CD pipeline
- Deployment configuration

## Acceptance Criteria

- Application compiles with `dotnet run`
- Loop demo shows iterative correction (fail → succeed)
- Graph demo shows DAG routing with visual arrows
- Governance demo shows human-in-the-loop checkpoint
- All output uses ConsoleLogger (no raw Console.WriteLine)
- Thread.Sleep delays for live audience viewing