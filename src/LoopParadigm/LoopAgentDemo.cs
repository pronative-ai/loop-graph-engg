namespace AgenticWorkflowConsole.LoopParadigm;

public static class LoopAgentDemo
{
    private static bool _compileSucceeded = false;

    public static async Task RunAsync()
    {
        ConsoleLogger.LoopBorder("LOOP ENGINEERING DEMO");
        ConsoleLogger.Info("Demonstrating autonomous iterative correction pattern");
        ConsoleLogger.Pause(1000);

        var iterations = new[] { 1, 2 };

        foreach (var iteration in iterations)
        {
            ConsoleLogger.LlmReasoning(iteration, "Analyzing project state...");
            ConsoleLogger.Pause(800);

            ConsoleLogger.ToolCall(iteration, "Invoking CompileProject tool...");
            ConsoleLogger.Pause(600);

            var result = CompileProject();
            ConsoleLogger.Observation(iteration, $"Tool result: {result}");

            if (_compileSucceeded)
            {
                ConsoleLogger.Success("Loop converged - project compiled successfully!");
                break;
            }

            ConsoleLogger.LlmReasoning(iteration, "Failure detected - initiating correction loop...");
            ConsoleLogger.Pause(1000);
        }

        ConsoleLogger.Pause(500);
    }

    private static string CompileProject()
    {
        if (!_compileSucceeded)
        {
            _compileSucceeded = true;
            return "ERROR: CS0246 - Type 'MissingType' not found";
        }

        return "Build succeeded. 0 warnings, 0 errors.";
    }
}