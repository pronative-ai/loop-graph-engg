namespace AgenticWorkflowConsole;

/// <summary>
/// Workflow Context passed to middleware and node interceptors during MAF workflow execution.
/// </summary>
public class WorkflowContext<TState>
{
    public TState State { get; set; } = default!;
    public string NextNode { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

/// <summary>
/// Middleware definition wrapping MAF workflow node executions.
/// </summary>
public class WorkflowMiddleware<TState>
{
    public Func<WorkflowContext<TState>, Func<Task>, Task> Execute { get; set; } = (context, next) => next();
}

/// <summary>
/// HIGHLIGHT: Microsoft Agent Framework (MAF) Native Workflow Engine
/// Constructs, validates, and executes multi-agent directed acyclic graphs using MAF's official
/// <see cref="Microsoft.Agents.AI.Workflows.WorkflowBuilder"/>, <see cref="Microsoft.Agents.AI.Workflows.Workflow"/>,
/// <see cref="FunctionExecutor{TInput, TOutput}"/>, and <see cref="InProcessExecution"/> runtime.
/// </summary>
public class AgenticWorkflow<TState> where TState : class, new()
{
    private readonly Dictionary<string, FunctionExecutor<TState, TState>> _executors = new();
    private readonly Dictionary<string, Func<TState, Task>> _handlers = new();
    private readonly List<(string Source, string Target)> _sequentialEdges = new();
    private readonly List<(string Source, string[] Targets)> _fanOutSplits = new();
    private readonly List<(string[] Sources, string Target)> _fanInJoins = new();
    private readonly List<(string Source, string Target, Func<TState, bool> Condition)> _conditionalEdges = new();
    private readonly List<WorkflowMiddleware<TState>> _middlewares = new();
    private readonly List<string> _terminalNodes = new();
    private readonly string _sessionId = Guid.NewGuid().ToString();
    private string? _initialNodeName;

    /// <summary>
    /// Registers the initial starting node of the MAF Workflow.
    /// </summary>
    public AgenticWorkflow<TState> AddInitialNode(string name, Func<TState, Task> execute)
    {
        _initialNodeName = name;
        RegisterNodeExecutor(name, execute);
        return this;
    }

    /// <summary>
    /// Registers a standard processing node in the MAF Workflow.
    /// </summary>
    public AgenticWorkflow<TState> AddNode(string name, Func<TState, Task> execute)
    {
        RegisterNodeExecutor(name, execute);
        return this;
    }

    /// <summary>
    /// Registers a terminal completion node in the MAF Workflow.
    /// </summary>
    public AgenticWorkflow<TState> AddTerminalNode(string name, Func<TState, Task>? execute = null)
    {
        _terminalNodes.Add(name);
        RegisterNodeExecutor(name, execute ?? (_ => Task.CompletedTask));
        return this;
    }

    /// <summary>
    /// HIGHLIGHT: MAF Native Parallel Split - Configures fan-out concurrency to child agent nodes.
    /// </summary>
    public AgenticWorkflow<TState> AddParallelSplit(string source, string[] targets)
    {
        _fanOutSplits.Add((source, targets));
        return this;
    }

    /// <summary>
    /// HIGHLIGHT: MAF Native Parallel Join - Configures fan-in barrier synchronization across completed branches.
    /// </summary>
    public AgenticWorkflow<TState> AddParallelJoin(string[] sources, string target)
    {
        _fanInJoins.Add((sources, target));
        return this;
    }

    /// <summary>
    /// HIGHLIGHT: MAF Native Conditional Edge - Evaluates state predicates before routing transitions.
    /// </summary>
    public AgenticWorkflow<TState> AddConditionalEdge(string source, string target, Func<TState, bool> condition)
    {
        _conditionalEdges.Add((source, target, condition));
        return this;
    }

    /// <summary>
    /// Registers a sequential directed edge between two workflow nodes.
    /// </summary>
    public AgenticWorkflow<TState> AddEdge(string source, string target)
    {
        _sequentialEdges.Add((source, target));
        return this;
    }

    /// <summary>
    /// HIGHLIGHT: MAF Governance Middleware - Injects guardrails and approval checkpoints into the pipeline.
    /// </summary>
    public AgenticWorkflow<TState> UseMiddleware(Func<WorkflowContext<TState>, Func<Task>, Task> middleware)
    {
        _middlewares.Add(new WorkflowMiddleware<TState> { Execute = middleware });
        return this;
    }

    /// <summary>
    /// Builds the official MAF <see cref="Microsoft.Agents.AI.Workflows.Workflow"/> from registered nodes and edges.
    /// </summary>
    public Workflow BuildMafWorkflow()
    {
        if (string.IsNullOrEmpty(_initialNodeName) || !_executors.TryGetValue(_initialNodeName, out var initialExecutor))
        {
            throw new InvalidOperationException("Workflow requires an initial starting node.");
        }

        // Initialize MAF WorkflowBuilder with starting executor
        var builder = new WorkflowBuilder(initialExecutor);

        // Add Sequential Edges
        foreach (var (source, target) in _sequentialEdges)
        {
            if (_executors.TryGetValue(source, out var srcEx) && _executors.TryGetValue(target, out var tgtEx))
            {
                builder.AddEdge(srcEx, tgtEx);
            }
        }

        // Add Parallel Fan-Out Edges
        foreach (var (source, targets) in _fanOutSplits)
        {
            if (_executors.TryGetValue(source, out var srcEx))
            {
                var targetExecutors = targets
                    .Where(_executors.ContainsKey)
                    .Select(t => (ExecutorBinding)_executors[t])
                    .ToList();

                if (targetExecutors.Count > 0)
                {
                    builder.AddFanOutEdge(srcEx, targetExecutors);
                }
            }
        }

        // Add Parallel Fan-In Barrier Edges
        foreach (var (sources, target) in _fanInJoins)
        {
            if (_executors.TryGetValue(target, out var tgtEx))
            {
                var sourceExecutors = sources
                    .Where(_executors.ContainsKey)
                    .Select(s => (ExecutorBinding)_executors[s])
                    .ToList();

                if (sourceExecutors.Count > 0)
                {
                    builder.AddFanInBarrierEdge(sourceExecutors, tgtEx);
                }
            }
        }

        // Add Conditional Edges
        foreach (var (source, target, condition) in _conditionalEdges)
        {
            if (_executors.TryGetValue(source, out var srcEx) && _executors.TryGetValue(target, out var tgtEx))
            {
                builder.AddEdge<TState>(srcEx, tgtEx, s => s != null && condition(s));
            }
        }

        // Configure Output Nodes
        var outputExecutors = _terminalNodes
            .Where(_executors.ContainsKey)
            .Select(t => (ExecutorBinding)_executors[t])
            .ToArray();

        if (outputExecutors.Length > 0)
        {
            builder.WithOutputFrom(outputExecutors);
        }

        return builder.Build();
    }

    /// <summary>
    /// Executes the workflow using MAF's official <see cref="InProcessExecution"/> engine,
    /// streaming execution events and applying telemetry attributes.
    /// </summary>
    public async Task ExecuteAsync(TState initialState)
    {
        var mafWorkflow = BuildMafWorkflow();

        // HIGHLIGHT: MAF Native Streaming Workflow Runner - Streams and processes workflow events in real time
        await using var run = await InProcessExecution.RunStreamingAsync(mafWorkflow, initialState);

        await foreach (var workflowEvent in run.WatchStreamAsync())
        {
            switch (workflowEvent)
            {
                case WorkflowStartedEvent:
                    ConsoleLogger.Info($"[MAF WORKFLOW] Initialized execution run ({_sessionId})");
                    break;

                case SuperStepStartedEvent:
                    // Superstep notification in MAF
                    break;

                case ExecutorInvokedEvent invoked:
                    ConsoleLogger.Info($"[MAF WORKFLOW] Invoking node: {invoked.ExecutorId}");
                    break;

                case ExecutorCompletedEvent completed:
                    ConsoleLogger.Info($"[MAF WORKFLOW] Completed node: {completed.ExecutorId}");
                    break;

                case WorkflowOutputEvent outputEvent:
                    ConsoleLogger.Success($"[MAF WORKFLOW] Reached output deliverable: {outputEvent.ExecutorId}");
                    break;
            }
        }
    }

    private void RegisterNodeExecutor(string name, Func<TState, Task> execute)
    {
        _handlers[name] = execute;

        // Wrap execution in MAF FunctionExecutor with middleware pipeline and OpenTelemetry tracing
        var executor = new FunctionExecutor<TState, TState>(
            name,
            async (state, context, cancellationToken) =>
            {
                using var nodeActivity = TelemetryConfiguration.ActivitySource.StartActivity($"Workflow.Node.{name}");
                nodeActivity?.SetTag("workflow.session_id", _sessionId);
                nodeActivity?.SetTag("workflow.node_name", name);

                if (state is CodingProjectState preState)
                {
                    nodeActivity?.SetTag("workflow.goal", preState.Goal);
                    if (!string.IsNullOrEmpty(preState.ArchitectureSpec)) nodeActivity?.SetTag("workflow.input_spec", preState.ArchitectureSpec);
                    if (!string.IsNullOrEmpty(preState.BackendCode)) nodeActivity?.SetTag("workflow.input_backend", preState.BackendCode);
                    if (!string.IsNullOrEmpty(preState.FrontendCode)) nodeActivity?.SetTag("workflow.input_frontend", preState.FrontendCode);
                }

                var workflowContext = new WorkflowContext<TState>
                {
                    State = state,
                    NextNode = name,
                    SessionId = _sessionId
                };

                await ExecuteMiddlewarePipeline(workflowContext, async () =>
                {
                    await execute(state);
                });

                if (state is CodingProjectState postState)
                {
                    if (!string.IsNullOrEmpty(postState.ArchitectureSpec)) nodeActivity?.SetTag("workflow.output_spec", postState.ArchitectureSpec);
                    if (!string.IsNullOrEmpty(postState.BackendCode)) nodeActivity?.SetTag("workflow.output_backend", postState.BackendCode);
                    if (!string.IsNullOrEmpty(postState.FrontendCode)) nodeActivity?.SetTag("workflow.output_frontend", postState.FrontendCode);
                    if (!string.IsNullOrEmpty(postState.ReviewNotes)) nodeActivity?.SetTag("workflow.output_review", postState.ReviewNotes);
                    if (!string.IsNullOrEmpty(postState.DeploymentLogs)) nodeActivity?.SetTag("workflow.output_deployment", postState.DeploymentLogs);
                }

                return state;
            });

        _executors[name] = executor;
    }

    private async Task ExecuteMiddlewarePipeline(WorkflowContext<TState> context, Func<Task> action)
    {
        var index = 0;

        async Task Next()
        {
            if (index < _middlewares.Count)
            {
                var middleware = _middlewares[index++];
                await middleware.Execute(context, Next);
            }
            else
            {
                await action();
            }
        }

        await Next();
    }
}
