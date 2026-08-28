using AgenticWorkflowConsole.Governance;
using AgenticWorkflowConsole.GraphParadigm;
using AgenticWorkflowConsole.LoopParadigm;
using AgenticWorkflowConsole.Shared;

namespace AgenticWorkflowConsole;

// Program is the console application entry point. It owns process-level setup
// (loading the .env file, initializing OpenTelemetry tracing, and building the shared LLM chat client)
// and then dispatches control to whichever walkthrough paradigm the user selects: Loop, Graph,
// or Governance.
internal static class Program
{
    // Shared across all walkthroughs so each reuses the same configured model/session.
    private static IChatClient? s_chatClient;

    static async Task Main(string[] args)
    {
        ConsoleLogger.BrandBanner();
        ConsoleLogger.Info("Initializing agentic runtime environment...");
        ConsoleLogger.Pause(1000);

        // Load .env values first
        Env.TraversePath().Load();

        // HIGHLIGHT: Runtime Initialization & OpenTelemetry Setup - Configures Langfuse export and distributed tracing before agent invocation
        using var tracerProvider = TelemetryConfiguration.InitializeTracerProvider();
        var otlpEndpoint = TelemetryConfiguration.LoadOtlpTraceEndpoint();
        ConsoleLogger.Success($"[MONITORING] OpenTelemetry trace export target -> {otlpEndpoint}");

        // HIGHLIGHT: LLM Gateway Connection - Initializes Microsoft Agent Framework IChatClient with Azure/OpenAI endpoint
        // Gateway setup is best-effort: if credentials are missing, we log a note and proceed
        try
        {
            var gatewayUrl = LlmConfiguration.LoadGatewayUrl();
            var modelName = LlmConfiguration.LoadModelName();
            s_chatClient = LlmConfiguration.CreateChatClient();

            ConsoleLogger.Success($"[GATEWAY] Connected to: {gatewayUrl}");
            ConsoleLogger.Success($"[MODEL] Using: {modelName}");

            using (var startupSpan = TelemetryConfiguration.ActivitySource.StartActivity("Runtime.Startup"))
            {
                startupSpan?.SetTag("service.name", TelemetryConfiguration.LoadServiceName());
                startupSpan?.SetTag("model.name", modelName);
                startupSpan?.SetTag("gateway.url", gatewayUrl);
            }
            TelemetryConfiguration.Flush();
        }
        catch (Exception ex)
        {
            ConsoleLogger.SecurityWarning($"Gateway init note: {ex.Message}");
            ConsoleLogger.Info("Continuing in direct execution mode.");
            ConsoleLogger.BlankLine();
        }

        // HIGHLIGHT: Paradigm Dispatcher Menu - Main presentation hub switching between Loop, Graph, and Governance paradigms
        // Each option demonstrates a distinct agentic architecture pattern using Microsoft Agent Framework.
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
                    using (TelemetryConfiguration.ActivitySource.StartActivity("Walkthrough.LoopEngineering"))
                    {
                        await LoopAgentWalkthrough.RunAsync(s_chatClient);
                    }
                    TelemetryConfiguration.Flush();
                    break;
                case "2":
                    using (TelemetryConfiguration.ActivitySource.StartActivity("Walkthrough.GraphEngineering"))
                    {
                        await GraphWorkflowWalkthrough.RunAsync(s_chatClient);
                    }
                    TelemetryConfiguration.Flush();
                    break;
                case "3":
                    using (TelemetryConfiguration.ActivitySource.StartActivity("Walkthrough.GovernanceMiddleware"))
                    {
                        await MiddlewareGuardrail.RunWithGuardrailAsync(s_chatClient);
                    }
                    TelemetryConfiguration.Flush();
                    break;
                case "4":
                    using (TelemetryConfiguration.ActivitySource.StartActivity("Walkthrough.RunAll"))
                    {
                        await RunAllWalkthroughs();
                    }
                    TelemetryConfiguration.Flush();
                    break;
                case "5":
                    TelemetryConfiguration.Flush();
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