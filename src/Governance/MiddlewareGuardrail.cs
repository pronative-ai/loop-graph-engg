namespace AgenticWorkflowConsole.Governance;

// Governance walkthrough entry point: shows how to inject a human-authorization gate
// (guardrail) into the AgenticWorkflow pipeline using its middleware hook.
// While normal graph execution flows node-to-node transparently, this middleware
// intercepts the transition to DeploymentNode and requires an operator to
// explicitly approve (or deny) before the pipeline may continue.
public static class MiddlewareGuardrail
{
    public static async Task RunWithGuardrailAsync(IChatClient? baseClient)
    {
        ConsoleLogger.GraphBorder("GOVERNANCE MIDDLEWARE WALKTHROUGH");
        ConsoleLogger.Info("Demonstrating human-in-the-loop checkpoint integrated via AgenticWorkflow middleware");
        ConsoleLogger.Pause(1000);

        var workflow = new AgenticWorkflow<CodingProjectState>();

        // Register the Governance Interceptor Middleware: this is the key guardrail.
        // It wraps every node hop, but only pauses execution when the target is the
        // deployment node - so ordinary nodes run through unobstructed while the most
        // dangerous transition gets a human checkpoint.
        workflow.UseMiddleware(async (context, next) =>
        {
            if (string.Equals(context.NextNode, "DeploymentNode", StringComparison.OrdinalIgnoreCase))
            {
                // HIGHLIGHT: The human-checkpoint gate for production deployment.
                // Middleware intercepts the transition, prompts the operator for
                // approval, and either allows the pipeline to proceed (pass) or
                // throws to halt the workflow (fail). This is the pass/fail path
                // to walk through live: show that "deny" blocks deployment.
                ConsoleLogger.BlankLine();
                ConsoleLogger.Info("[Middleware] Guardrail triggered: Intercepting transition to 'DeploymentNode'");
                ConsoleLogger.SecurityWarning("CRITICAL: PRODUCTION DEPLOYMENT AUTHORIZATION REQUIRED");
                ConsoleLogger.BlankLine();

                await HumanCheckpointStore.TriggerApprovalPrompt(context.SessionId);

                Console.Write("👉 Press ENTER to authorize deployment, or type 'deny' to reject: ");
                var input = Console.ReadLine();

                if (string.Equals(input?.Trim(), "deny", StringComparison.OrdinalIgnoreCase))
                {
                    HumanCheckpointStore.Reject(context.SessionId);
                    ConsoleLogger.BlankLine();
                    ConsoleLogger.SecurityWarning("Action Blocked: Deployment was DENIED by operator.");
                    throw new UnauthorizedAccessException("Operator checkpoint denied transition to DeploymentNode.");
                }

                HumanCheckpointStore.Approve(context.SessionId);
                ConsoleLogger.Success("[Middleware] Authorization confirmed: Resuming execution pipeline.");
                ConsoleLogger.BlankLine();
            }

            await next();
        });

        ChatClientAgent? deploymentManager = null;
        if (baseClient != null)
        {
            deploymentManager = new ChatClientAgent(
                chatClient: baseClient,
                instructions: """
                    You are a release and deployment engineer.
                    Summarize the deployment manifest and release verification steps (max 100 words).
                    """,
                name: "DeploymentManagerAgent",
                description: "Manages build packaging and deployment verification");
        }

        workflow.AddInitialNode("PrepareReleaseNode", async state =>
        {
            ConsoleLogger.Info("[PrepareReleaseNode] Generating release manifest and artifact bundle...");

            if (deploymentManager != null)
            {
                var sb = new StringBuilder();
                await foreach (var update in deploymentManager.RunStreamingAsync(
                    "Prepare release summary for production deployment of the Task Manager Service.",
                    session: null))
                {
                    if (!string.IsNullOrEmpty(update.Text))
                    {
                        ConsoleLogger.StreamToken(update.Text);
                        sb.Append(update.Text);
                    }
                }
                ConsoleLogger.BlankLine();
                state.DeploymentLogs = sb.ToString();
            }
            else
            {
                state.DeploymentLogs = "Release manifest generated: Image tag v1.0.0, SHA256 verified.";
                ConsoleLogger.Success("[PrepareReleaseNode] Staged release candidate v1.0.0");
            }

            ConsoleLogger.Pause(500);
        });

        workflow.AddEdge("PrepareReleaseNode", "DeploymentNode");

        workflow.AddTerminalNode("DeploymentNode", state =>
        {
            ConsoleLogger.Info("[DeploymentNode] Executing zero-downtime deployment rollout...");
            ConsoleLogger.Pause(500);
            ConsoleLogger.Success("✓ [DeploymentNode] Production deployment completed successfully!");
            return Task.CompletedTask;
        });

        var state = new CodingProjectState
        {
            Goal = "Deploy verified task management service to production"
        };

        try
        {
            await workflow.ExecuteAsync(state);
            ConsoleLogger.BlankLine();
            ConsoleLogger.Success("✓ Governance guardrail workflow execution finished successfully!");
        }
        catch (UnauthorizedAccessException ex)
        {
            ConsoleLogger.BlankLine();
            ConsoleLogger.SecurityWarning($"Workflow halted by security guardrail: {ex.Message}");
        }
        catch (Exception ex)
        {
            ConsoleLogger.BlankLine();
            ConsoleLogger.SecurityWarning($"Workflow encountered unexpected exception: {ex.Message}");
        }

        ConsoleLogger.Pause(500);
    }
}