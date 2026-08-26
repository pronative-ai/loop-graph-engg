namespace AgenticWorkflowConsole.Governance;

public static class MiddlewareGuardrail
{
    public static async Task RunWithGuardrailAsync(IChatClient? baseClient)
    {
        ConsoleLogger.GraphBorder("GOVERNANCE MIDDLEWARE DEMO");
        ConsoleLogger.Info("Demonstrating human-in-the-loop checkpoint with real LLM agent");
        ConsoleLogger.Pause(1000);

        if (baseClient == null)
        {
            ConsoleLogger.SecurityWarning("No LLM client available - running in mock mode");
            await RunMockAsync();
            return;
        }

        try
        {
            await RunLlmAsync(baseClient);
        }
        catch (Exception ex)
        {
            ConsoleLogger.SecurityWarning($"LLM call failed: {ex.Message}");
            ConsoleLogger.Info("Falling back to mock mode for demonstration...");
            ConsoleLogger.Pause(500);
            ConsoleLogger.BlankLine();
            await RunMockAsync();
        }
    }

    private static async Task RunLlmAsync(IChatClient baseClient)
    {
        var agent = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are a deployment manager. When asked to prepare a deployment, provide a summary
                of what will be deployed. When deployment is approved, confirm it. When denied,
                acknowledge and suggest next steps.
                Keep responses concise (under 100 words).
                """,
            name: "DeploymentManager",
            description: "Manages deployment approvals and rollouts");

        ConsoleLogger.Info("[Middleware] Active - intercepting deployment authorization");
        ConsoleLogger.Pause(500);

        ConsoleLogger.Info("[DeploymentManager] Preparing deployment plan...");
        await foreach (var update in agent.RunStreamingAsync(
            "Prepare a deployment plan for the task management API to production.",
            session: null))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                ConsoleLogger.StreamToken(update.Text);
            }
        }

        ConsoleLogger.BlankLine();
        ConsoleLogger.BlankLine();
        ConsoleLogger.SecurityWarning("CRITICAL: DEPLOYMENT AUTHORIZATION REQUIRED");
        ConsoleLogger.BlankLine();
        Console.Write("👉 Press ENTER to authorize deployment, or type 'deny' to reject: ");
        var input = Console.ReadLine();

        if (string.Equals(input, "deny", StringComparison.OrdinalIgnoreCase))
        {
            ConsoleLogger.SecurityWarning("Deployment DENIED by operator");
            ConsoleLogger.Pause(500);
            ConsoleLogger.Info("[DeploymentManager] Acknowledging denial...");
            await foreach (var denyUpdate in agent.RunStreamingAsync(
                "Deployment was DENIED by the operator. Acknowledge and explain next steps.",
                session: null))
            {
                if (!string.IsNullOrEmpty(denyUpdate.Text))
                {
                    ConsoleLogger.StreamToken(denyUpdate.Text);
                }
            }
            ConsoleLogger.BlankLine();
        }
        else
        {
            ConsoleLogger.Success("[Middleware] Authorization granted - releasing deployment");
            ConsoleLogger.Pause(500);
            ConsoleLogger.Info("[DeploymentManager] Confirming deployment...");
            await foreach (var approveUpdate in agent.RunStreamingAsync(
                "Deployment has been APPROVED. Confirm the rollout and summarize what was deployed.",
                session: null))
            {
                if (!string.IsNullOrEmpty(approveUpdate.Text))
                {
                    ConsoleLogger.StreamToken(approveUpdate.Text);
                }
            }
            ConsoleLogger.BlankLine();
        }

        ConsoleLogger.BlankLine();
        ConsoleLogger.Success("[Middleware] Guardrail checkpoint complete");
    }

    private static async Task RunMockAsync()
    {
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
        Console.Write("👉 Press ENTER to authorize deployment, or type 'deny' to reject: ");
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