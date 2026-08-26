# Design: Rename Namespace and Consolidate Using Statements

## Approach

Two independent changes applied across all source files.

## Namespace Rename

| Current | New |
|---------|-----|
| `AksAgenticWorkflowConsole` | `AgenticWorkflowConsole` |

Update locations:
- `src/Program.cs` (namespace declaration)
- `src/LlmConfiguration.cs` (namespace declaration)
- `src/WorkflowGraph.cs` (namespace declaration)
- `src/TerminalExecutionTool.cs` (namespace declaration)
- `src/HumanCheckpointStore.cs` (namespace declaration)
- `src/CodingProjectState.cs` (namespace declaration)

## Global Usings

Create `src/GlobalUsings.cs` with all unique using statements collected from across the codebase:

```csharp
global using Azure;
global using Azure.AI.OpenAI;
global using Azure.Identity;
global using DotNetEnv;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Text;
```

Remove individual `using` statements from:
- `src/Program.cs` (4 usings)
- `src/LlmConfiguration.cs` (3 usings)
- `src/WorkflowGraph.cs` (1 using)
- `src/TerminalExecutionTool.cs` (2 usings)
- `src/HumanCheckpointStore.cs` (1 using)

## Files Modified

- `src/GlobalUsings.cs` (new) - consolidated using statements
- `src/Program.cs` - remove usings, rename namespace
- `src/LlmConfiguration.cs` - remove usings, rename namespace
- `src/WorkflowGraph.cs` - remove usings, rename namespace
- `src/TerminalExecutionTool.cs` - remove usings, rename namespace
- `src/HumanCheckpointStore.cs` - remove usings, rename namespace
- `src/CodingProjectState.cs` - rename namespace