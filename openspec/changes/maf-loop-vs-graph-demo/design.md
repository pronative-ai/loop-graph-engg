# Design: Agent Framework Loop vs Graph Presentation Demo

## Architecture

```
src/
├── Program.cs                          # Entry point orchestration
├── LoopParadigm/
│   └── LoopAgentDemo.cs               # Autonomous loop with iterative correction
├── GraphParadigm/
│   └── GraphWorkflowDemo.cs           # DAG workflow with node routing
├── Governance/
│   └── MiddlewareGuardrail.cs         # Human-in-the-loop middleware
└── Shared/
    └── ConsoleLogger.cs               # Centralized visual formatting
```

## Package Dependencies

- `Microsoft.Agents.AI` - AIAgent, Workflow abstractions
- `Microsoft.Extensions.AI` - IChatClient interface
- `Microsoft.Extensions.AI.OpenAI` - Mock client implementation

## Component Design

### ConsoleLogger

Centralized static class handling:
- Color-coded text output ( ConsoleColor routing)
- Border rendering (single-line Yellow, double-line Magenta)
- Arrow visualization for graph routing
- Tree branch rendering for parallel fan-out
- Security warning block with DarkRed background

### LoopAgentDemo

Mock `AIAgent` implementation demonstrating:
- `CompileProject` tool registration
- First iteration: tool returns failure
- Second iteration: tool returns success
- Visual logging of each loop phase

### GraphWorkflowDemo

Mock `Workflow` implementation demonstrating:
- Node registration (Architect, Coder, Deployment)
- DAG edge configuration
- Sequential execution with visual arrows
- Parallel fan-out with tree branch rendering

### MiddlewareGuardrail

Mock middleware demonstrating:
- Interception on Deployment node
- Console pause for human authorization
- High-visibility security warning block
- Resume after authorization

## Visual Styling

| Layer | Border | Color |
|-------|--------|-------|
| Graph | `[===]` double-line | Magenta |
| Loop | `[---]` single-line | Yellow |
| LLM Reasoning | `[Loop #X] [LLM REASONING]` | Blue |
| Tool Call | `[Loop #X] [TOOL CALL]` | Cyan |
| Observation | `[Loop #X] [OBSERVATION]` | DarkGray |
| Security Warning | `!! [CRITICAL...] !!` | DarkRed bg, White text |
| Tree branches | `├──` `└──` | DarkCyan |