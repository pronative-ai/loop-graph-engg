namespace AgenticWorkflowConsole.GraphParadigm;

public static class GraphWorkflowDemo
{
    public static async Task RunAsync(IChatClient? baseClient)
    {
        ConsoleLogger.GraphBorder("GRAPH ENGINEERING DEMO");
        ConsoleLogger.Info("Demonstrating end-to-end DAG workflow with multi-agent orchestration");
        ConsoleLogger.Pause(1000);

        if (baseClient == null)
        {
            ConsoleLogger.SecurityWarning("No active LLM client configured. Running direct deterministic graph orchestration...");
            await RunDeterministicGraphAsync();
            return;
        }

        try
        {
            await RunAgenticGraphAsync(baseClient);
        }
        catch (Exception ex)
        {
            ConsoleLogger.BlankLine();
            ConsoleLogger.SecurityWarning($"Graph LLM execution error: {ex.Message}");
            ConsoleLogger.Info("Executing deterministic graph fallback...");
            await RunDeterministicGraphAsync();
        }
    }

    private static async Task RunAgenticGraphAsync(IChatClient baseClient)
    {
        var architect = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are a senior software architect. Given the goal, produce a concise system architecture 
                specifying endpoints, data models, and component responsibilities (max 150 words).
                """,
            name: "ArchitectAgent",
            description: "Produces high-level architectural specifications");

        var backendCoder = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are a backend C# engineer. Given the architecture spec, produce the backend API 
                controller and repository signatures (max 150 words).
                """,
            name: "BackendCoderAgent",
            description: "Implements backend microservices and domain logic");

        var frontendCoder = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are a frontend UI engineer. Given the architecture spec, produce the UI client 
                components and state models (max 150 words).
                """,
            name: "FrontendCoderAgent",
            description: "Implements frontend user interfaces");

        var reviewer = new ChatClientAgent(
            chatClient: baseClient,
            instructions: """
                You are a lead code reviewer. Evaluate the backend and frontend components. 
                State whether the implementation meets the architectural spec and confirm approval (max 100 words).
                """,
            name: "ReviewerAgent",
            description: "Audits and verifies multi-agent code deliverables");

        var workflow = new AgenticWorkflow<CodingProjectState>();

        // 1. Initial Node: Architect
        workflow.AddInitialNode("ArchitectNode", async state =>
        {
            ConsoleLogger.Info("[ArchitectNode] Generating architecture specification...");
            var sb = new StringBuilder();
            await foreach (var update in architect.RunStreamingAsync(
                $"Project Goal: {state.Goal}\nProduce the architectural blueprint.",
                session: null))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    ConsoleLogger.StreamToken(update.Text);
                    sb.Append(update.Text);
                }
            }
            ConsoleLogger.BlankLine();
            state.ArchitectureSpec = sb.ToString();
            state.TasksCreated = true;
            ConsoleLogger.Success("[ArchitectNode] Architecture specification generated.");
            ConsoleLogger.Pause(500);
        });

        // 2. Parallel Split: BackendCoder & FrontendCoder
        workflow.AddParallelSplit("ArchitectNode", ["BackendCoderNode", "FrontendCoderNode"]);

        workflow.AddNode("BackendCoderNode", async state =>
        {
            ConsoleLogger.Info("[BackendCoderNode] Synthesizing backend endpoints and services...");
            var sb = new StringBuilder();
            await foreach (var update in backendCoder.RunStreamingAsync(
                $"Architecture:\n{state.ArchitectureSpec}\nImplement backend controllers.",
                session: null))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    sb.Append(update.Text);
                }
            }
            state.BackendCode = sb.ToString();
            ConsoleLogger.Success($"[BackendCoderNode] Backend implementation complete ({state.BackendCode.Length} chars).");
        });

        workflow.AddNode("FrontendCoderNode", async state =>
        {
            ConsoleLogger.Info("[FrontendCoderNode] Synthesizing UI components and views...");
            var sb = new StringBuilder();
            await foreach (var update in frontendCoder.RunStreamingAsync(
                $"Architecture:\n{state.ArchitectureSpec}\nImplement frontend UI views.",
                session: null))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    sb.Append(update.Text);
                }
            }
            state.FrontendCode = sb.ToString();
            ConsoleLogger.Success($"[FrontendCoderNode] Frontend implementation complete ({state.FrontendCode.Length} chars).");
        });

        // 3. Parallel Join: ReviewerNode
        workflow.AddParallelJoin(["BackendCoderNode", "FrontendCoderNode"], "ReviewerNode");

        workflow.AddNode("ReviewerNode", async state =>
        {
            ConsoleLogger.Info("[ReviewerNode] Evaluating code artifacts against architectural contract...");
            var sb = new StringBuilder();
            await foreach (var update in reviewer.RunStreamingAsync(
                $"Backend:\n{state.BackendCode}\n\nFrontend:\n{state.FrontendCode}\n\nReview the combined solution and approve.",
                session: null))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    ConsoleLogger.StreamToken(update.Text);
                    sb.Append(update.Text);
                }
            }
            ConsoleLogger.BlankLine();
            state.ReviewNotes = sb.ToString();
            state.IsApproved = true;
            ConsoleLogger.Success("[ReviewerNode] Code review complete: Approved for deployment.");
            ConsoleLogger.Pause(500);
        });

        // 4. Conditional Edge: to DeploymentNode
        workflow.AddConditionalEdge("ReviewerNode", "DeploymentNode", state => state.IsApproved);

        // 5. Terminal Node: Deployment
        workflow.AddTerminalNode("DeploymentNode", state =>
        {
            ConsoleLogger.Info("[DeploymentNode] Finalizing release package and verifying manifest...");
            state.DeploymentLogs = "Release package compiled and verified.";
            ConsoleLogger.Success("✓ [DeploymentNode] Deployment payload staged successfully.");
            return Task.CompletedTask;
        });

        var initialState = new CodingProjectState
        {
            Goal = "Build a distributed AI task coordination service with REST API and Web Dashboard"
        };

        ConsoleLogger.Info($"[DAG Orchestration] Starting workflow execution for goal: '{initialState.Goal}'");
        ConsoleLogger.BlankLine();

        await workflow.ExecuteAsync(initialState);

        ConsoleLogger.BlankLine();
        ConsoleLogger.Success("✓ Graph DAG workflow completed with synchronized parallel branches!");
        ConsoleLogger.Pause(500);
    }

    private static async Task RunDeterministicGraphAsync()
    {
        var workflow = new AgenticWorkflow<CodingProjectState>();

        workflow.AddInitialNode("ArchitectNode", state =>
        {
            ConsoleLogger.Info("[ArchitectNode] Generated distributed microservices blueprint.");
            state.ArchitectureSpec = "Service Architecture: Gateway -> TaskManager -> Storage";
            state.TasksCreated = true;
            return Task.CompletedTask;
        });

        workflow.AddParallelSplit("ArchitectNode", ["BackendCoderNode", "FrontendCoderNode"]);

        workflow.AddNode("BackendCoderNode", state =>
        {
            state.BackendCode = "public class TaskController : ControllerBase { ... }";
            ConsoleLogger.Success("[BackendCoderNode] C# Backend services compiled.");
            return Task.CompletedTask;
        });

        workflow.AddNode("FrontendCoderNode", state =>
        {
            state.FrontendCode = "<TaskListComponent ... />";
            ConsoleLogger.Success("[FrontendCoderNode] UI components rendered.");
            return Task.CompletedTask;
        });

        workflow.AddParallelJoin(["BackendCoderNode", "FrontendCoderNode"], "ReviewerNode");

        workflow.AddNode("ReviewerNode", state =>
        {
            state.ReviewNotes = "All contracts match specifications.";
            state.IsApproved = true;
            ConsoleLogger.Success("[ReviewerNode] Code audit passed (100% compliance).");
            return Task.CompletedTask;
        });

        workflow.AddConditionalEdge("ReviewerNode", "DeploymentNode", state => state.IsApproved);

        workflow.AddTerminalNode("DeploymentNode", state =>
        {
            state.DeploymentLogs = "Release deployed.";
            ConsoleLogger.Success("✓ [DeploymentNode] Release verified.");
            return Task.CompletedTask;
        });

        var state = new CodingProjectState { Goal = "Deterministic demo workflow" };
        await workflow.ExecuteAsync(state);
    }
}