using AgenticWorkflowConsole.Governance;
using AgenticWorkflowConsole.GraphParadigm;
using AgenticWorkflowConsole.LoopParadigm;
using AgenticWorkflowConsole.Shared;

namespace AgenticWorkflowConsole;

// Program is the console application entry point. It owns process-level setup
// (loading the .env file and building the shared LLM chat client) and then
// dispatches control to whichever walkthrough paradigm the user selects: Loop, Graph,
// or Governance. The heavy lifting lives in the walkthrough classes themselves.
internal static class Program
{
    // Shared across all walkthroughs so each reuses the same configured model/session.
    private static IChatClient? s_chatClient;

    static async Task Main(string[] args)
    {
        ConsoleLogger.BrandBanner();
        ConsoleLogger.Info("Initializing agentic runtime environment...");
        ConsoleLogger.Pause(1000);

        // Gateway setup is best-effort: if the .env or credentials are missing,
        // we log a warning and keep going in direct execution mode.
        try
        {
            // Load .env values first, then construct the single shared chat client.
            Env.TraversePath().Load();
            var gatewayUrl = LlmConfiguration.LoadGatewayUrl();
            var modelName = LlmConfiguration.LoadModelName();
            s_chatClient = LlmConfiguration.CreateChatClient();

            ConsoleLogger.Success($"[GATEWAY] Connected to: {gatewayUrl}");
            ConsoleLogger.Success($"[MODEL] Using: {modelName}");
        }
        catch (Exception ex)
        {
            ConsoleLogger.SecurityWarning($"Gateway init note: {ex.Message}");
            ConsoleLogger.Info("Continuing in direct execution mode.");
            ConsoleLogger.BlankLine();
        }

        // HIGHLIGHT: The presentation start anchor. This interactive dispatch
        // loop lets the audience pick the Live Loop, Graph, or Governance walkthrough.
        // Each walkthrough illustrates a different agent-orchestration paradigm.
        while (true)
        {
            ConsoleLogger.BlankLine();
            ConsoleLogger.Highlight("=== Select an Agentic Walkthrough ===");
            ConsoleLogger.MenuOption("1", "Loop Engineering Walkthrough (Iterative Self-Correction)");
            ConsoleLogger.MenuOption("2", "Graph Engineering Walkthrough (DAG Multi-Agent Routing)");
            ConsoleLogger.MenuOption("3", "Governance Middleware Walkthrough (Human Checkpoint Guardrail)");
            ConsoleLogger.MenuOption("4", "Run All Walkthroughs Sequentially");
            ConsoleLogger.MenuOption("5", "Exit");
            ConsoleLogger.BlankLine();
            ConsoleLogger.StreamToken("Enter your choice (1-5): ", ConsoleColor.Yellow);

            var choice = Console.ReadLine();
            ConsoleLogger.BlankLine();

            switch (choice)
            {
                case "1":
                    await LoopAgentWalkthrough.RunAsync(s_chatClient);
                    break;
                case "2":
                    await GraphWorkflowWalkthrough.RunAsync(s_chatClient);
                    break;
                case "3":
                    await MiddlewareGuardrail.RunWithGuardrailAsync(s_chatClient);
                    break;
                case "4":
                    await RunAllWalkthroughs();
                    break;
                case "5":
                    ConsoleLogger.Success("Thank you for exploring AI Agent Workflows with pronative.ai. Goodbye!");
                    return;
                default:
                    ConsoleLogger.SecurityWarning("Invalid choice. Please enter 1-5.");
                    break;
            }
        }
    }

    private static async Task RunAllWalkthroughs()
    {
        await LoopAgentWalkthrough.RunAsync(s_chatClient);
        ConsoleLogger.BlankLine();
        await GraphWorkflowWalkthrough.RunAsync(s_chatClient);
        ConsoleLogger.BlankLine();
        await MiddlewareGuardrail.RunWithGuardrailAsync(s_chatClient);
    }
}