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
            if (!string.IsNullOrWhiteSpace(update.Text))
            {
                ConsoleLogger.Info(update.Text);
            }
        }

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
                if (!string.IsNullOrWhiteSpace(denyUpdate.Text))
                {
                    ConsoleLogger.Info(denyUpdate.Text);
                }
            }
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
                if (!string.IsNullOrWhiteSpace(approveUpdate.Text))
                {
                    ConsoleLogger.Info(approveUpdate.Text);
                }
            }
        }

        ConsoleLogger.BlankLine();
        ConsoleLogger.Success("[Middleware] Guardrail checkpoint complete");
        await Task.CompletedTask;
    }

    private static async Task RunMockAsync()
    {
        await Task.CompletedTask;
    }
}