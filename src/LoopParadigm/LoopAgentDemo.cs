namespace AgenticWorkflowConsole.LoopParadigm;

public static class LoopAgentDemo
{
    private static int s_iteration = 1;
    private static readonly TerminalExecutionTool s_terminalTool = new();

    public static async Task RunAsync(IChatClient? baseClient)
    {
        ConsoleLogger.LoopBorder("LOOP ENGINEERING DEMO");
        ConsoleLogger.Info("Demonstrating autonomous iterative correction via ChatClientAgent with live build tools");
        ConsoleLogger.Pause(1000);

        s_iteration = 1;

        if (baseClient == null)
        {
            ConsoleLogger.SecurityWarning("No active LLM client configured. Running direct live build verification...");
            await RunLiveBuildVerificationAsync();
            return;
        }

        var compileTool = AIFunctionFactory.Create(
            async (string? context) => await ExecuteLiveBuildCheckAsync(context),
            "CompileProject",
            "Compiles the current .NET solution/project using dotnet build and returns detailed diagnostic outputs.");

        var agent = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are a senior .NET compiler engineer and autonomous loop agent.
                Your task is to inspect the project build status using the CompileProject tool.
                
                Steps:
                1. Call the CompileProject tool to run the live compiler.
                2. Analyze the compiler output and diagnostics.
                3. If the build succeeds, summarize the status cleanly and announce completion.
                4. If any warnings or errors occur, explain the resolution strategy and verify again.
                
                Always be concise, precise, and professional.
                """,
            name: "LoopDevAgent",
            description: "An autonomous developer agent executing iterative build diagnostics and correction",
            tools: [compileTool]);

        ConsoleLogger.Info("[LoopDevAgent] Starting autonomous correction loop with live tool invocation...");
        ConsoleLogger.BlankLine();

        try
        {
            ConsoleLogger.LlmReasoning(s_iteration, "Initiating autonomous build verification cycle...");
            
            await foreach (var update in agent.RunStreamingAsync(
                "Run the CompileProject tool now to verify the current codebase and provide an engineering assessment.",
                session: null))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    ConsoleLogger.StreamToken(update.Text);
                }
            }

            ConsoleLogger.BlankLine();
            ConsoleLogger.BlankLine();
            ConsoleLogger.Success("✓ Loop converged: Autonomous verification cycle completed successfully!");
        }
        catch (Exception ex)
        {
            ConsoleLogger.BlankLine();
            ConsoleLogger.SecurityWarning($"LLM invocation encountered an issue: {ex.Message}");
            ConsoleLogger.Info("Executing direct live compiler verification fallback...");
            await RunLiveBuildVerificationAsync();
        }

        ConsoleLogger.Pause(500);
    }

    private static async Task<string> ExecuteLiveBuildCheckAsync(string? context)
    {
        ConsoleLogger.BlankLine();
        ConsoleLogger.ToolCall(s_iteration, $"Executing live tool [CompileProject] (Iteration #{s_iteration})...");
        
        var buildResult = await s_terminalTool.RunBuildVerificationAsync();
        
        ConsoleLogger.Observation(s_iteration++, $"Compiler Output received ({buildResult.Split('\n').Length} lines)");
        ConsoleLogger.BlankLine();

        return buildResult;
    }

    private static async Task RunLiveBuildVerificationAsync()
    {
        ConsoleLogger.ToolCall(1, "Invoking live TerminalExecutionTool (dotnet build)...");
        var result = await s_terminalTool.RunBuildVerificationAsync();
        ConsoleLogger.Observation(1, result);
        ConsoleLogger.BlankLine();
        ConsoleLogger.Success("✓ Live build verification completed!");
    }
}