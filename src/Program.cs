using AgenticWorkflowConsole.Governance;
using AgenticWorkflowConsole.GraphParadigm;
using AgenticWorkflowConsole.LoopParadigm;
using AgenticWorkflowConsole.Shared;

namespace AgenticWorkflowConsole;

internal static class Program
{
    static async Task Main(string[] args)
    {
        ConsoleLogger.Info("=== Microsoft Agent Framework Demo ===");
        ConsoleLogger.Info("Loop Engineering vs Graph Engineering");
        ConsoleLogger.Pause(500);

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
                    await LoopAgentDemo.RunAsync();
                    break;
                case "2":
                    await GraphWorkflowDemo.RunAsync();
                    break;
                case "3":
                    await MiddlewareGuardrail.RunWithGuardrailAsync();
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
        await LoopAgentDemo.RunAsync();
        ConsoleLogger.BlankLine();
        await GraphWorkflowDemo.RunAsync();
        ConsoleLogger.BlankLine();
        await MiddlewareGuardrail.RunWithGuardrailAsync();
    }
}