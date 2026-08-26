## 1. Project Setup

- [ ] 1.1 Add Microsoft.Extensions.AI package to existing .NET 10 project
- [ ] 1.2 Update GlobalUsings.cs with Microsoft.Extensions.AI imports
- [ ] 1.3 Create subdirectories: src/LoopParadigm, src/GraphParadigm, src/Governance, src/Shared

## 2. ConsoleLogger Implementation

- [ ] 2.1 Create src/Shared/ConsoleLogger.cs with color-coded text methods
- [ ] 2.2 Implement border rendering: `[===]` double-line (Magenta) and `[---]` single-line (Yellow)
- [ ] 2.3 Implement arrow visualization for graph routing
- [ ] 2.4 Implement tree branch rendering for parallel fan-out
- [ ] 2.5 Implement security warning block with DarkRed background

## 3. Loop Paradigm Implementation

- [ ] 3.1 Create src/LoopParadigm/LoopAgentDemo.cs with mock AIAgent
- [ ] 3.2 Implement CompileProject tool with fail/succeed logic
- [ ] 3.3 Implement loop iteration logging with `[Loop #X]` headers
- [ ] 3.4 Add Thread.Sleep delays between iterations

## 4. Graph Paradigm Implementation

- [ ] 4.1 Create src/GraphParadigm/GraphWorkflowDemo.cs with mock Workflow
- [ ] 4.2 Implement ArchitectAgent and CoderAgent nodes
- [ ] 4.3 Implement DAG edge configuration and execution
- [ ] 4.4 Add visual arrow rendering between nodes
- [ ] 4.5 Add parallel fan-out visualization with tree branches

## 5. Governance Middleware Implementation

- [ ] 5.1 Create src/Governance/MiddlewareGuardrail.cs with middleware interceptor
- [ ] 5.2 Implement deployment node interception logic
- [ ] 5.3 Add high-visibility security warning block
- [ ] 5.4 Implement console pause for human authorization
- [ ] 5.5 Add ASCII hand pointer to prompt

## 6. Program Entry Point

- [ ] 6.1 Update src/Program.cs with demo orchestration
- [ ] 6.2 Implement user selection (Loop, Graph, or Both)
- [ ] 6.3 Wire up all demos with ConsoleLogger
- [ ] 6.4 Add initialization and completion messages

## 7. Verification

- [ ] 7.1 Verify build succeeds with `dotnet build`
- [ ] 7.2 Verify application runs with `dotnet run`
- [ ] 7.3 Verify no raw Console.WriteLine in logic files