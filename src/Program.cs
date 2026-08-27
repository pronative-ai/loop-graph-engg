using AgenticWorkflowConsole.Governance;
using AgenticWorkflowConsole.GraphParadigm;
using AgenticWorkflowConsole.LoopParadigm;
using AgenticWorkflowConsole.Shared;

namespace AgenticWorkflowConsole;

// Program is the console application entry point. It owns process-level setup
// (loading the .env file and building the shared LLM chat client) and then
// dispatches control to whichever demo paradigm the user selects: Loop, Graph,
// or Governance. The heavy lifting lives in the demo classes themselves.
internal static class Program
{
    // Shared across all demos so each reuses the same configured model/session.
    private static IChatClient? s_chatClient;

    static async Task Main(string[] args)
    {
        ConsoleLogger.Info("=== Microsoft Agent Framework Demo ===");
        ConsoleLogger.Info("Loop Engineering vs Graph Engineering (Real Orchestration)");
        ConsoleLogger.Pause(500);

        // Gateway setup is best-effort: if the .env or credentials are missing,
        // we log a warning and keep going in deterministic (offline) mode.
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
        // loop lets the audience pick the Live Loop, Graph, or Governance demo.
        // Each demo illustrates a different agent-orchestration paradigm.
        while (true)
        {
            ConsoleLogger.BlankLine();
            ConsoleLogger.Info("Select a demo to run:");
            ConsoleLogger.Info("  1. Loop Engineering Demo");
            ConsoleLogger.Info("  2. Graph Engineering Demo");
            ConsoleLogger.Info("  3. Governance Middleware Demo");
            ConsoleLogger.Info("  4. Run All Demos");
            ConsoleLogger.Info("  5. Exit");
            ConsoleLogger.BlankLine();
            Console.Write("Enter your choice (1-5): ");

            var choice = Console.ReadLine();
            ConsoleLogger.BlankLine();

            switch (choice)
            {
                case "1":
                    await LoopAgentDemo.RunAsync(s_chatClient);
                    break;
                case "2":
                    await GraphWorkflowDemo.RunAsync(s_chatClient);
                    break;
                case "3":
                    await MiddlewareGuardrail.RunWithGuardrailAsync(s_chatClient);
                    break;
                case "4":
                    await RunAllDemos();
                    break;
                case "5":
                    ConsoleLogger.Success("Goodbye!");
                    return;
                default:
                    ConsoleLogger.Info("Invalid choice. Please enter 1-5.");
                    break;
            }
        }
    }

    private static async Task RunAllDemos()
    {
        await LoopAgentDemo.RunAsync(s_chatClient);
        ConsoleLogger.BlankLine();
        await GraphWorkflowDemo.RunAsync(s_chatClient);
        ConsoleLogger.BlankLine();
        await MiddlewareGuardrail.RunWithGuardrailAsync(s_chatClient);
    }
}