namespace AgenticWorkflowConsole.Governance;

public static class MiddlewareGuardrail
{
    public static async Task RunWithGuardrailAsync()
    {
        ConsoleLogger.GraphBorder("GOVERNANCE MIDDLEWARE DEMO");
        ConsoleLogger.Info("Demonstrating human-in-the-loop checkpoint via middleware interceptor");
        ConsoleLogger.Pause(1000);

        await RunArchitectNode();
        ConsoleLogger.Pause(500);

        await RunCoderNode();
        ConsoleLogger.Pause(500);

        await InterceptDeployment();

        ConsoleLogger.Success("Deployment authorized - proceeding with rollout!");
        ConsoleLogger.Pause(500);
    }

    private static async Task RunArchitectNode()
    {
        ConsoleLogger.Info("[ArchitectNode] Analyzing requirements...");
        ConsoleLogger.Pause(800);
        ConsoleLogger.Success("[ArchitectNode] Design complete");
        ConsoleLogger.Arrow("ArchitectNode", "CoderNode");
        await Task.CompletedTask;
    }

    private static async Task RunCoderNode()
    {
        ConsoleLogger.Info("[CoderNode] Implementing code...");
        ConsoleLogger.Pause(1000);
        ConsoleLogger.Success("[CoderNode] Implementation complete");
        ConsoleLogger.Arrow("CoderNode", "DeploymentNode");
        await Task.CompletedTask;
    }

    private static async Task InterceptDeployment()
    {
        ConsoleLogger.Info("[Middleware] Intercepting deployment action...");
        ConsoleLogger.Pause(500);

        ConsoleLogger.SecurityWarning("CRITICAL: DEPLOYMENT AUTHORIZATION REQUIRED");

        ConsoleLogger.BlankLine();
        Console.WriteLine("👉 Press ENTER to authorize deployment, or type 'deny' to reject:");
        var input = Console.ReadLine();

        if (string.Equals(input, "deny", StringComparison.OrdinalIgnoreCase))
        {
            ConsoleLogger.SecurityWarning("Deployment DENIED by operator");
            return;
        }

        ConsoleLogger.Success("[Middleware] Authorization received - releasing deployment");
        await Task.CompletedTask;
    }
}