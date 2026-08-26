namespace AgenticWorkflowConsole.GraphParadigm;

public static class GraphWorkflowDemo
{
    public static async Task RunAsync()
    {
        ConsoleLogger.GraphBorder("GRAPH ENGINEERING DEMO");
        ConsoleLogger.Info("Demonstrating DAG workflow with specialized micro-agents");
        ConsoleLogger.Pause(1000);

        await RunArchitectNode();
        ConsoleLogger.Pause(500);

        await RunCoderNode();
        ConsoleLogger.Pause(500);

        await RunParallelTests();
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
        ConsoleLogger.Success("[CoderNode] Code complete - emitting to TestNodes");
        ConsoleLogger.Arrow("CoderNode", "UnitTestNode");
        ConsoleLogger.Arrow("CoderNode", "IntegrationTestNode");
        await Task.CompletedTask;
    }

    private static async Task RunParallelTests()
    {
        ConsoleLogger.Info("Executing parallel test suites:");
        ConsoleLogger.TreeBranch("", "UnitTestNode - Running unit tests...");
        ConsoleLogger.Pause(600);
        ConsoleLogger.TreeBranch("", "IntegrationTestNode - Running integration tests...", true);
        ConsoleLogger.Pause(800);
        ConsoleLogger.Success("All test suites passed!");
        ConsoleLogger.Arrow("TestNodes", "DeploymentNode");
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