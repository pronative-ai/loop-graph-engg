## 1. Program Entry & Configuration

- [x] 1.1 Add a responsibility comment to `Program.cs` and comment the top-level entry flow (app setup and dispatch to Loop/Graph demos).
- [x] 1.2 Add a `// HIGHLIGHT: <note>` marker to the entry point that kicks off the workflow, indicating the presentation start anchor.
- [x] 1.3 Add comments to `LlmConfiguration.cs` explaining how gateway URL/key map to the MAF client config.

## 2. Loop vs Graph Paradigms

- [x] 2.1 Add comments to `LoopParadigm/LoopAgentDemo.cs` covering the iterative loop intent and any non-obvious control flow.
- [x] 2.2 Add a `// HIGHLIGHT:` marker on the loop iteration core, noting it as a key side-by-side comparison point.
- [x] 2.3 Add comments to `GraphParadigm/GraphWorkflowDemo.cs` covering graph/node/edge wiring.
- [x] 2.4 Add a `// HIGHLIGHT:` marker on the parallel or conditional routing section for the demo contrast.

## 3. Graph & Orchestration Core

- [x] 3.1 Add a responsibility comment to `WorkflowGraph.cs` and comment node/edge definitions and conditional routing.
- [x] 3.2 Add a `// HIGHLIGHT:` marker on the planner→[backend,frontend]→reviewer→deploy flow as the primary architecture visual.

## 4. Tools, Governance & Persistence

- [x] 4.1 Add comments to `TerminalExecutionTool.cs` covering tool registration and execution flow.
- [x] 4.2 Add a `// HIGHLIGHT:` marker on the tool-invocation method for the presenter to demonstrate external tool use.
- [x] 4.3 Add comments to `Governance/MiddlewareGuardrail.cs` covering guardrail interception and the human checkpoint gate.
- [x] 4.4 Add a `// HIGHLIGHT:` marker on the deployment human-checkpoint guardrail pass/fail path.
- [x] 4.5 Add comments to `HumanCheckpointStore.cs` and `Shared/ConsoleLogger.cs` covering persistence and logging responsibilities.
- [x] 4.6 Add comments to `CodingProjectState.cs` and `GlobalUsings.cs` noting their role (state model; global using consolidation).

## 5. Verification

- [x] 5.1 Run `dotnet build src/AksAgenticWorkflowConsole.csproj` and confirm it completes without warnings/errors.
- [x] 5.2 Run `dotnet test` (or `make test`) and confirm all existing tests pass.
- [x] 5.3 Search `src` for `HIGHLIGHT` and confirm markers read as a coherent top-to-bottom presentation path with a short note each.
- [x] 5.4 Reviewer pass: confirm comments are intent-focused (no verbatim code restatements) and style is uniform.
