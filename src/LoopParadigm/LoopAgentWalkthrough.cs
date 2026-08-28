namespace AgenticWorkflowConsole.LoopParadigm;

/// <summary>
/// HIGHLIGHT: Microsoft Agent Framework (MAF) Official LoopAgent Walkthrough
/// Demonstrates the official <see cref="Microsoft.Agents.AI.LoopAgent"/> from Microsoft Agent Framework
/// as documented in https://learn.microsoft.com/en-us/agent-framework/agents/looping?pivots=programming-language-csharp
/// wrapping an autonomous developer <see cref="ChatClientAgent"/> with a <see cref="LoopEvaluator"/> and <see cref="LoopAgentOptions"/>.
/// </summary>
public static class LoopAgentWalkthrough
{
    public static async Task RunAsync(IChatClient? baseClient)
    {
        ConsoleLogger.LoopBorder("LOOP ENGINEERING WALKTHROUGH");
        ConsoleLogger.Info("Demonstrating autonomous iterative correction via official Microsoft Agent Framework (MAF) LoopAgent");
        ConsoleLogger.Pause(1000);

        var workspace = new LoopDiagnosticWorkspace();

        if (baseClient == null)
        {
            ConsoleLogger.SecurityWarning("No active LLM client configured. Please configure your gateway in .env to run live Loop agent.");
            return;
        }

        /* -------------------------------------------------------------------------
         * STAGE 1: Tool Registration with AIFunctionFactory
         * Converts local C# workspace diagnostics and file operations into typed
         * LLM tools equipped with OpenTelemetry tracing tags.
         * ------------------------------------------------------------------------- */

        // HIGHLIGHT: Tool Registration with AIFunctionFactory - Exposes deterministic workspace inspection and compilation functions as LLM tools
        var inspectTool = AIFunctionFactory.Create(
            () =>
            {
                using var activity = TelemetryConfiguration.ActivitySource.StartActivity("Tool.InspectCode");
                int iter = Math.Max(1, workspace.IterationCount);
                activity?.SetTag("loop.iteration", iter);
                activity?.SetTag("tool.name", "InspectCode");
                activity?.SetTag("gen_ai.tool.name", "InspectCode");
                activity?.SetTag("gen_ai.tool.input", $"targetFileName={workspace.TargetFileName}");

                ConsoleLogger.BlankLine();
                ConsoleLogger.ToolCall(iter, $"Executing live tool [InspectCode] (Iteration #{iter}) ({workspace.TargetFileName})...");
                var code = workspace.InspectCode();

                activity?.SetTag("gen_ai.tool.output", code);
                activity?.SetTag("gen_ai.tool.is_success", true);

                ConsoleLogger.Observation(iter, $"Source code inspection returned {code.Split('\n').Length} lines.");
                ConsoleLogger.BlankLine();
                return code;
            },
            "InspectCode",
            "Inspects the target C# source code file in the workspace.");

        var patchTool = AIFunctionFactory.Create(
            (string? updatedCode, string? explanation) =>
            {
                using var activity = TelemetryConfiguration.ActivitySource.StartActivity("Tool.ApplyCodeFix");
                int iter = Math.Max(1, workspace.IterationCount);
                var safeExplanation = explanation ?? string.Empty;
                var safeCode = updatedCode ?? string.Empty;

                activity?.SetTag("loop.iteration", iter);
                activity?.SetTag("tool.name", "ApplyCodeFix");
                activity?.SetTag("gen_ai.tool.name", "ApplyCodeFix");
                activity?.SetTag("gen_ai.tool.input", $"explanation={safeExplanation}, patchLength={safeCode.Length}");
                activity?.SetTag("code.explanation", safeExplanation);

                ConsoleLogger.BlankLine();
                ConsoleLogger.ToolCall(iter, $"Executing live tool [ApplyCodeFix] (Iteration #{iter}) - {safeExplanation}...");
                var result = workspace.ApplyCodeFix(safeCode, safeExplanation);

                activity?.SetTag("gen_ai.tool.output", result);
                activity?.SetTag("gen_ai.tool.is_success", true);

                ConsoleLogger.Observation(iter, result);
                ConsoleLogger.BlankLine();
                return result;
            },
            "ApplyCodeFix",
            "Applies a C# code correction or patch to the workspace file and prepares it for verification.");

        var compileTool = AIFunctionFactory.Create(
            async () =>
            {
                using var activity = TelemetryConfiguration.ActivitySource.StartActivity("Tool.CompileAndVerify");
                int iter = workspace.IterationCount + 1;
                activity?.SetTag("loop.iteration", iter);
                activity?.SetTag("tool.name", "CompileAndVerify");
                activity?.SetTag("gen_ai.tool.name", "CompileAndVerify");
                activity?.SetTag("gen_ai.tool.input", $"targetFileName={workspace.TargetFileName}, iteration={iter}");

                ConsoleLogger.BlankLine();
                ConsoleLogger.ToolCall(iter, $"Executing live tool [CompileAndVerify] (Iteration #{iter})...");
                var output = await workspace.CompileAndVerifyAsync(baseClient);

                activity?.SetTag("gen_ai.tool.output", output);
                activity?.SetTag("gen_ai.tool.is_success", workspace.IsClean);

                ConsoleLogger.Observation(iter, output);
                ConsoleLogger.BlankLine();
                return output;
            },
            "CompileAndVerify",
            "Compiles and validates the C# workspace using the real-time .NET compiler and static analysis engine.");

        /* -------------------------------------------------------------------------
         * STAGE 2: Base AIAgent Definition
         * Defines the autonomous agent persona, operational instructions, and tool bindings.
         * ------------------------------------------------------------------------- */

        // HIGHLIGHT: Autonomous ChatClientAgent Definition - Instantiates single-agent loop engineer armed with tools and diagnostic protocol
        var baseAgent = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are an autonomous senior .NET compiler engineer and Loop Agent.
                Your mission is to autonomously diagnose, fix, and verify the C# project in your workspace.
                
                Loop Engineering Protocol:
                1. First, call CompileAndVerify to execute the compiler engine and observe diagnostics.
                2. Call InspectCode to view the complete source code and analyze defects.
                3. Call ApplyCodeFix with your complete updated C# code and an explanation.
                4. Call CompileAndVerify to re-test the updated code.
                5. If any errors or warnings remain, repeat the loop.
                6. When the build succeeds with 0 errors and 0 warnings (STATUS: [PASS - VERIFIED]), announce convergence with STATUS: [PASS - VERIFIED].
                
                Always be concise, precise, and professional.
                """,
            name: "LoopDevAgent",
            description: "An autonomous developer agent executing iterative build diagnostics and correction",
            tools: [inspectTool, patchTool, compileTool]);

        /* -------------------------------------------------------------------------
         * STAGE 3: Microsoft Agent Framework (MAF) LoopAgent Composition
         * Wraps base agent with official LoopAgent, LoopEvaluator, and LoopAgentOptions
         * as specified in https://learn.microsoft.com/en-us/agent-framework/agents/looping
         * ------------------------------------------------------------------------- */

        // HIGHLIGHT: MAF LoopEvaluator - Evaluates completion criteria and supplies feedback for the next loop iteration
        var loopEvaluator = new DelegateLoopEvaluator((context, cancellationToken) =>
        {
            if (workspace.IsClean)
            {
                return ValueTask.FromResult(LoopEvaluation.Stop());
            }

            int nextIteration = workspace.IterationCount + 1;
            ConsoleLogger.Pause(800);
            return ValueTask.FromResult(LoopEvaluation.Continue(
                $"[Loop #{nextIteration}] The build is not clean yet. Inspect the latest compiler output with CompileAndVerify, apply fixes using ApplyCodeFix, and verify until 0 errors and 0 warnings are achieved."));
        });

        // HIGHLIGHT: MAF Official LoopAgent - Directly instantiates Microsoft.Agents.AI.LoopAgent with bounded max iterations
        AIAgent loopAgent = new LoopAgent(
            baseAgent,
            loopEvaluator,
            new LoopAgentOptions
            {
                MaxIterations = 10
            });

        ConsoleLogger.Info("[LoopAgent] Executing official Microsoft.Agents.AI.LoopAgent streaming iteration run...");
        ConsoleLogger.BlankLine();

        /* -------------------------------------------------------------------------
         * STAGE 4: Stream Official MAF LoopAgent Execution
         * Invokes the LoopAgent, streaming token reasoning and tool invocations across all iterations.
         * ------------------------------------------------------------------------- */

        try
        {
            using var loopRunActivity = TelemetryConfiguration.ActivitySource.StartActivity("MAF.LoopAgent.Run");
            loopRunActivity?.SetTag("gen_ai.agent.name", "LoopAgent");
            loopRunActivity?.SetTag("gen_ai.base_agent", "LoopDevAgent");

            string initialPrompt = "Start the autonomous correction loop now. Call CompileAndVerify to observe the initial build state, diagnose defects, apply fixes, and iterate until 0 errors and 0 warnings are achieved.";

            await foreach (var update in loopAgent.RunStreamingAsync(initialPrompt, session: null))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    ConsoleLogger.StreamToken(update.Text);
                }
            }

            int totalIterations = Math.Max(1, workspace.IterationCount);
            ConsoleLogger.BlankLine();
            ConsoleLogger.BlankLine();
            ConsoleLogger.Success($"✓ Loop converged: Official MAF LoopAgent verification cycle completed in {totalIterations} iterations!");
        }
        catch (Exception ex)
        {
            ConsoleLogger.BlankLine();
            ConsoleLogger.SecurityWarning($"LLM invocation encountered an issue: {ex.Message}");
        }
        finally
        {
            TelemetryConfiguration.Flush();
        }

        ConsoleLogger.Pause(500);
    }
}