using System.Text;
using AgenticWorkflowConsole.Shared;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgenticWorkflowConsole.LoopParadigm;

// The Loop paradigm: a single autonomous agent that keeps iterating over the same
// problem (observe real-time LLM compiler diagnostics -> inspect -> apply patch -> re-verify)
// under its own direction until it converges cleanly.
// In this walkthrough, the agent and the compiler evaluator perform authentic, live LLM
// interactions with progressive iteration tracking:
//   Loop #1: Live compiler engine evaluates code -> detects errors (CS0103, CS1002).
//   Loop #2: Dev agent applies syntax patch -> compiler engine detects warnings (CS8602 nullability).
//   Loop #3: Dev agent applies null-safety & defensive guards -> compiler engine verifies 0 errors & 0 warnings.
public static class LoopAgentWalkthrough
{
    public static async Task RunAsync(IChatClient? baseClient)
    {
        ConsoleLogger.LoopBorder("LOOP ENGINEERING WALKTHROUGH");
        ConsoleLogger.Info("Demonstrating autonomous iterative correction via ChatClientAgent with realtime LLM verification");
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
         * STAGE 2: ChatClientAgent Instantiation
         * Defines the autonomous agent persona, operational instructions, and tool bindings.
         * ------------------------------------------------------------------------- */

        // HIGHLIGHT: Autonomous ChatClientAgent Definition - Instantiates single-agent loop engineer armed with tools and diagnostic protocol
        var agent = new ChatClientAgent(
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
                6. When the build succeeds with 0 errors and 0 warnings (STATUS: [PASS - VERIFIED]), announce convergence.
                
                Always be concise, precise, and professional.
                """,
            name: "LoopDevAgent",
            description: "An autonomous developer agent executing iterative build diagnostics and correction",
            tools: [inspectTool, patchTool, compileTool]);

        ConsoleLogger.Info("[LoopDevAgent] Starting autonomous correction loop with live tool invocation...");
        ConsoleLogger.BlankLine();

        /* -------------------------------------------------------------------------
         * STAGE 3: Autonomous Feedback Iteration Loop
         * Repeatedly invokes the agent, streaming its reasoning and tool executions
         * until the workspace signals clean compilation or max iterations reached.
         * ------------------------------------------------------------------------- */

        // HIGHLIGHT: Autonomous Feedback Iteration Loop - Streams reasoning and tool execution iteratively until 0 errors / 0 warnings convergence
        try
        {
            const int maxIterations = 5;
            string prompt = "Start the autonomous correction loop now. Call CompileAndVerify to observe the initial build state, diagnose defects, apply fixes, and iterate until 0 errors and 0 warnings are achieved.";

            while (workspace.IterationCount < maxIterations && !workspace.IsClean)
            {
                int currentLoop = Math.Max(1, workspace.IterationCount == 0 ? 1 : workspace.IterationCount + 1);
                using var loopActivity = TelemetryConfiguration.ActivitySource.StartActivity($"Loop.Iteration.{currentLoop}");
                loopActivity?.SetTag("loop.iteration", currentLoop);
                loopActivity?.SetTag("gen_ai.agent.name", "LoopDevAgent");
                loopActivity?.SetTag("gen_ai.prompt", prompt);

                ConsoleLogger.LlmReasoning(currentLoop, $"[Loop #{currentLoop}] Autonomous Agent reasoning and executing corrective action...");

                var responseCollector = new StringBuilder();
                await foreach (var update in agent.RunStreamingAsync(prompt, session: null))
                {
                    if (!string.IsNullOrEmpty(update.Text))
                    {
                        ConsoleLogger.StreamToken(update.Text);
                        responseCollector.Append(update.Text);
                    }
                }

                loopActivity?.SetTag("gen_ai.response", responseCollector.ToString());
                loopActivity?.SetTag("loop.is_clean", workspace.IsClean);

                if (workspace.IsClean)
                {
                    break;
                }

                int nextLoop = workspace.IterationCount + 1;
                prompt = $"[Loop #{nextLoop}] Continue autonomous loop. Inspect the latest compiler output with CompileAndVerify, resolve any remaining warnings or test failures using ApplyCodeFix, and verify until clean.";
                ConsoleLogger.Pause(800);
            }

            int totalIterations = Math.Max(1, workspace.IterationCount);
            ConsoleLogger.BlankLine();
            ConsoleLogger.BlankLine();
            ConsoleLogger.Success($"✓ Loop converged: Autonomous verification cycle completed successfully in {totalIterations} iterations!");
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