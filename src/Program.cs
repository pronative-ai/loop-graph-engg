namespace AgenticWorkflowConsole;

internal static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== AKS Agentic Workflow Console ===");
        Console.WriteLine();

        try
        {
            // Load .env file into environment variables
            Env.Load();

            // Validate environment variables
            ValidateEnvironmentVariables();

            // Load configuration
            var gatewayUrl = LlmConfiguration.LoadGatewayUrl();
            var modelName = LlmConfiguration.LoadModelName();

            Console.WriteLine($"[CONFIG] Gateway URL: {gatewayUrl}");
            Console.WriteLine($"[CONFIG] Model: {modelName}");
            Console.WriteLine();

            // Create LLM client
            var chatClient = LlmConfiguration.CreateChatClient();

            // Create terminal execution tool
            var terminalTool = new TerminalExecutionTool();

            // Create agents
            Console.WriteLine("[AGENTS] Creating AI agents...");

            // Note: In a real implementation, you would use chatClient.AsAIAgent()
            // For this demonstration, we're showing the workflow structure
            Console.WriteLine("[AGENTS] BackendCoder agent created");
            Console.WriteLine("[AGENTS] FrontendCoder agent created");
            Console.WriteLine();

            // Build the workflow graph
            Console.WriteLine("[WORKFLOW] Building workflow graph...");
            var workflow = BuildWorkflowGraph(terminalTool);

            // Create initial state
            var projectState = new CodingProjectState
            {
                Goal = "Build a secure e-commerce billing portal."
            };

            Console.WriteLine($"[WORKFLOW] Goal: {projectState.Goal}");
            Console.WriteLine();

            // Execute the workflow
            Console.WriteLine("[WORKFLOW] Starting workflow execution...");
            Console.WriteLine();

            await workflow.ExecuteAsync(projectState);

            Console.WriteLine();
            Console.WriteLine("[WORKFLOW] Workflow execution completed successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Configuration error: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Please set the required environment variables.");
            Console.WriteLine("See .env.example for configuration details.");
            Environment.Exit(1);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Authorization error: {ex.Message}");
            Console.ResetColor();
            Environment.Exit(2);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Unexpected error: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(3);
        }
    }

    private static void ValidateEnvironmentVariables()
    {
        Console.WriteLine("[CONFIG] Validating environment variables...");

        var requiredVariables = new[] { "GATEWAY_URL", "GATEWAY_KEY" };
        var missingVariables = new List<string>();

        foreach (var variable in requiredVariables)
        {
            var value = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrWhiteSpace(value))
            {
                missingVariables.Add(variable);
            }
        }

        if (missingVariables.Count > 0)
        {
            throw new InvalidOperationException(
                $"Missing required environment variables: {string.Join(", ", missingVariables)}. " +
                "Please copy .env.example to .env and fill in your values.");
        }

        Console.WriteLine("[CONFIG] Environment variables validated.");
    }

    private static AgenticWorkflow<CodingProjectState> BuildWorkflowGraph(
        TerminalExecutionTool terminalTool)
    {
        var workflow = new AgenticWorkflow<CodingProjectState>();

        // Add initial Planner node
        workflow.AddInitialNode("Planner", async (state) =>
        {
            Console.WriteLine("[PLANNER] Breaking down user prompt into sub-tasks...");
            Console.WriteLine("[PLANNER] Creating backend and frontend tasks...");
            state.TasksCreated = true;
            Console.WriteLine("[PLANNER] Tasks created successfully.");
        });

        // Add BackendCoder agent node
        workflow.AddNode("BackendCoder", async (state) =>
        {
            Console.WriteLine("[BACKEND] Backend Coder agent starting...");
            Console.WriteLine("[BACKEND] Writing minimal Web APIs...");

            // Simulate terminal execution
            var result = await terminalTool.ExecuteAsync("dotnet --version");
            if (result.Success)
            {
                Console.WriteLine($"[BACKEND] .NET version: {result.Output}");
            }

            Console.WriteLine("[BACKEND] Backend coding completed.");
        });

        // Add FrontendCoder agent node
        workflow.AddNode("FrontendCoder", async (state) =>
        {
            Console.WriteLine("[FRONTEND] Frontend Blazor developer starting...");
            Console.WriteLine("[FRONTEND] Building clean UI components...");
            Console.WriteLine("[FRONTEND] Frontend coding completed.");
        });

        // Add Reviewer node
        workflow.AddNode("Reviewer", async (state) =>
        {
            Console.WriteLine("[REVIEWER] Evaluating code compilation...");
            Console.WriteLine("[REVIEWER] Checking code quality...");
            Console.WriteLine("[REVIEWER] Merging outputs...");
            state.IsApproved = true;
            Console.WriteLine("[REVIEWER] Code approved for deployment.");
        });

        // Add terminal Deployment node
        workflow.AddTerminalNode("Deployment", async (state) =>
        {
            Console.WriteLine("[DEPLOYMENT] Starting deployment process...");
            Console.WriteLine("[DEPLOYMENT] Deploying to production environment...");
            Console.WriteLine("[DEPLOYMENT] Deployment completed successfully.");
        });

        // Add parallel split from Planner to Backend/Frontend
        workflow.AddParallelSplit("Planner", new[] { "BackendCoder", "FrontendCoder" });

        // Add parallel join from Backend/Frontend to Reviewer
        workflow.AddParallelJoin(new[] { "BackendCoder", "FrontendCoder" }, "Reviewer");

        // Add conditional edge from Reviewer to Deployment
        workflow.AddConditionalEdge("Reviewer", "Deployment", state => state.IsApproved);

        // Add deployment checkpoint middleware
        workflow.UseMiddleware(async (context, next) =>
        {
            if (context.NextNode == "Deployment")
            {
                Console.WriteLine("[GUARDRAIL] Deployment node triggered.");
                Console.WriteLine("[GUARDRAIL] Pausing workflow for Human Verification.");

                // Trigger approval prompt
                await HumanCheckpointStore.TriggerApprovalPrompt(context.SessionId);

                // Wait for approval
                if (!await HumanCheckpointStore.WaitForApprovalAsync(context.SessionId))
                {
                    throw new UnauthorizedAccessException(
                        "Deployment rejected by platform administrator.");
                }

                Console.WriteLine("[GUARDRAIL] Deployment approved. Proceeding...");
            }

            await next();
        });

        return workflow;
    }
}
