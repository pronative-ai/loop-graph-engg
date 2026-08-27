## 1. Tool Input and Output Telemetry Instrumentation

- [x] 1.1 Update `InspectCode`, `ApplyCodeFix`, and `CompileAndVerify` in `src/LoopParadigm/LoopAgentWalkthrough.cs` to set span tags (`gen_ai.tool.input`, `gen_ai.tool.output`, `gen_ai.tool.is_success`) on tool activity spans
- [x] 1.2 Add structured console output in `src/LoopParadigm/LoopAgentWalkthrough.cs` for tool arguments and returned feedback

## 2. Workflow Graph Node Payload Instrumentation

- [x] 2.1 Update `src/WorkflowGraph.cs` to capture state snapshot before/after node executions and attach `workflow.node.input` and `workflow.node.output` span attributes
- [x] 2.2 Instrument Governance guardrail in `src/Governance/MiddlewareGuardrail.cs` to record operator decisions (`guardrail.operator_action`, `guardrail.reason`) on activity spans

## 3. Unit Testing & Verification

- [x] 3.1 Add unit tests in `tests/AksAgenticWorkflowConsole.Tests/` to verify tool and workflow node span attributes and tags are properly attached
- [x] 3.2 Run `make test` and verify 100% passing test suite
