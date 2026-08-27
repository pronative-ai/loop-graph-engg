## 1. Diagnostic Engine & Workspace

- [x] 1.1 Create `src/LoopParadigm/LoopDiagnosticWorkspace.cs` modeling progressive diagnostic states (Initial Error -> Warning -> Clean Build) with code inspection and verification capabilities
- [x] 1.2 Create unit tests in `tests/AksAgenticWorkflowConsole.Tests/LoopDiagnosticTests.cs` validating diagnostic state transitions and fix evaluations

## 2. MAF Tools & Multi-Iteration Loop Agent

- [x] 2.1 Register typed inspection, patch, and compile verification tools using `AIFunctionFactory` in `src/LoopParadigm/LoopAgentWalkthrough.cs`
- [x] 2.2 Implement the multi-iteration loop in `LoopAgentWalkthrough.cs` using real MAF `ChatClientAgent` streaming, driving at least 2-4 iterations (Error -> Warning -> Clean Convergence)
- [x] 2.3 Format iteration headers (`[Loop #X] [LLM REASONING]`, `[Loop #X] [TOOL CALL]`, `[Loop #X] [OBSERVATION]`) and streaming tokens cleanly

## 3. Verification & Validation

- [x] 3.1 Build `src/AksAgenticWorkflowConsole.csproj` and run all unit tests
- [x] 3.2 Confirm zero warnings, zero errors, and 100% test pass rate
