namespace AgenticWorkflowConsole.GraphParadigm;

public static class GraphWorkflowDemo
{
    public static async Task RunAsync(IChatClient? baseClient)
    {
        ConsoleLogger.GraphBorder("GRAPH ENGINEERING DEMO");
        ConsoleLogger.Info("Demonstrating DAG workflow with real ChatClientAgents");
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
        var architect = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are a software architect. Given a request, produce a high-level system design
                including architecture decisions and component breakdown.
                Keep responses concise (under 200 words).
                """,
            name: "ArchitectAgent",
            description: "Software architect that designs system architecture");

        var coder = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are a software developer. Given a system design, produce the implementation plan
                with specific classes, methods, and key logic.
                Keep responses concise (under 200 words).
                """,
            name: "CoderAgent",
            description: "Developer that implements system designs");

        ConsoleLogger.Arrow("ArchitectAgent", "CoderAgent");
        ConsoleLogger.Pause(500);
        ConsoleLogger.Info("Executing workflow...");
        ConsoleLogger.BlankLine();

        ConsoleLogger.Info("[ArchitectAgent] Running...");
        await foreach (var update in architect.RunStreamingAsync(
            "Design a REST API for a task management system with CRUD operations.",
            session: null))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                ConsoleLogger.StreamToken(update.Text);
            }
        }

        ConsoleLogger.BlankLine();
        ConsoleLogger.Pause(500);
        ConsoleLogger.BlankLine();
        ConsoleLogger.Arrow("ArchitectAgent", "CoderAgent");
        ConsoleLogger.BlankLine();

        ConsoleLogger.Info("[CoderAgent] Running...");
        await foreach (var update in coder.RunStreamingAsync(
            "Implement the system design from the architect. Provide specific classes, controllers, services, and repositories with dependency injection.",
            session: null))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                ConsoleLogger.StreamToken(update.Text);
            }
        }

        ConsoleLogger.BlankLine();
        ConsoleLogger.Success("Graph workflow completed successfully!");
        ConsoleLogger.Pause(500);
    }

    private static async Task RunMockAsync()
    {
        await RunArchitectNode();
        ConsoleLogger.Pause(500);
        await RunCoderNode();
        ConsoleLogger.Pause(500);
        await RunDeploymentNode();
        ConsoleLogger.Success("Graph workflow completed successfully!");
        ConsoleLogger.Pause(500);
    }

    private static async Task RunArchitectNode()
    {
        ConsoleLogger.Info("[ArchitectNode] Analyzing requirements...");
        ConsoleLogger.Pause(800);
        ConsoleLogger.Info("[ArchitectNode] Generating system design...");
        ConsoleLogger.Pause(600);
        ConsoleLogger.Success("[ArchitectNode] Design complete - emitting to CoderNode");
        ConsoleLogger.Arrow("ArchitectNode", "CoderNode");
        await Task.CompletedTask;
    }

    private static async Task RunCoderNode()
    {
        ConsoleLogger.Info("[CoderNode] Receiving architecture spec...");
        ConsoleLogger.Pause(800);
        ConsoleLogger.Info("[CoderNode] Implementing components...");
        ConsoleLogger.Pause(1000);
        ConsoleLogger.Success("[CoderNode] Code complete");
        ConsoleLogger.Arrow("CoderNode", "DeploymentNode");
        await Task.CompletedTask;
    }

    private static async Task RunDeploymentNode()
    {
        ConsoleLogger.Info("[DeploymentNode] Preparing deployment...");
        ConsoleLogger.Pause(800);
        ConsoleLogger.Success("[DeploymentNode] Deployment initiated!");
        await Task.CompletedTask;
    }
}