namespace AgenticWorkflowConsole.LoopParadigm;

public static class LoopAgentDemo
{
    private static int s_compileAttempts;
    private static int s_iteration;

    public static async Task RunAsync(IChatClient? baseClient)
    {
        ConsoleLogger.LoopBorder("LOOP ENGINEERING DEMO");
        ConsoleLogger.Info("Demonstrating autonomous iterative correction via ChatClientAgent");
        ConsoleLogger.Pause(1000);

        if (baseClient == null)
        {
            ConsoleLogger.SecurityWarning("No LLM client available - running in mock mode");
            await RunMockAsync();
            return;
        }

        s_compileAttempts = 0;
        s_iteration = 1;

        var tool = AIFunctionFactory.Create(CompileProject, "CompileProject",
            "Compiles the current .NET project and returns the build output. Errors indicate compilation failures.");

        var agent = new ChatClientAgent(
                    chatClient: baseClient,
                    instructions: """
                You are a senior .NET developer. You must fix a compilation error in the project.

                IMPORTANT: Call the CompileProject tool to check the build status. If it fails,
                analyze the error and propose a specific code fix. Then call CompileProject again
                to verify your fix. Continue until the build succeeds.

                Keep your responses concise and focused on the compilation task.
                """,
                    name: "DevAgent",
                    description: "A developer agent that fixes compilation errors",
                    tools: [tool]);

        ConsoleLogger.Info("[DevAgent] Starting autonomous loop...");
        ConsoleLogger.BlankLine();

        try
        {
            await foreach (var update in agent.RunStreamingAsync(
                "Fix the build. Run CompileProject to check the status.",
                session: null))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    ConsoleLogger.StreamToken(update.Text);
                }
            }

            ConsoleLogger.BlankLine();
            ConsoleLogger.Success("Loop converged - project compiled successfully!");
        }
        catch (Exception ex)
        {
            ConsoleLogger.BlankLine();
            ConsoleLogger.SecurityWarning($"LLM call failed: {ex.Message}");
            ConsoleLogger.Info("Falling back to mock mode for demonstration...");
            ConsoleLogger.Pause(500);
            await RunMockAsync();
            return;
        }
        ConsoleLogger.Pause(500);
    }

    private static async Task RunMockAsync()
    {
        var iterations = new[] { 1, 2 };

        foreach (var iteration in iterations)
        {
            ConsoleLogger.LlmReasoning(iteration, "Analyzing project state...");
            ConsoleLogger.Pause(800);

            ConsoleLogger.ToolCall(iteration, "Invoking CompileProject tool...");
            ConsoleLogger.Pause(600);

            var result = CompileProject();
            ConsoleLogger.Observation(iteration, $"Tool result: {result}");

            if (result.StartsWith("Build succeeded"))
            {
                ConsoleLogger.Success("Loop converged - project compiled successfully!");
                break;
            }

            ConsoleLogger.LlmReasoning(iteration, "Failure detected - initiating correction loop...");
            ConsoleLogger.Pause(1000);
        }

        ConsoleLogger.Pause(500);
        await Task.CompletedTask;
    }

    [Description("Compiles the current .NET project and returns the build output")]
    private static string CompileProject()
    {
        s_compileAttempts++;
        ConsoleLogger.BlankLine();
        ConsoleLogger.ToolCall(s_iteration, $"CompileProject attempt #{s_compileAttempts}");

        string result;
        if (s_compileAttempts == 1)
        {
            result = "ERROR: CS0246 - The type or namespace name 'MissingType' could not be found.";
        }
        else
        {
            result = "Build succeeded. 0 warnings, 0 errors.";
        }

        ConsoleLogger.Observation(s_iteration++, $"Tool result: {result}");
        ConsoleLogger.BlankLine();
        return result;
    }
}