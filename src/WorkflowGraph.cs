namespace AgenticWorkflowConsole;

// AgenticWorkflow is a small hand-rolled graph engine that lets walkthroughs wire a DAG
// of nodes (each with an async action), connect them with edges, and layer
// middleware over every node execution. Node/edge definitions below are plain
// data holders; AgenticWorkflow.ExecuteAsync is the traversal + routing engine.
// The GraphParadigm and Governance walkthroughs build on these primitives.

// A single unit of work in the graph. ExecuteAsync is the action run when the
// node is reached; IsTerminal marks the node as an allowed stopping point.
public class WorkflowNode<TState>
{
    public string Name { get; set; } = string.Empty;
    public Func<TState, Task>? ExecuteAsync { get; set; }
    public bool IsTerminal { get; set; }
}

// Connects two nodes. IsParallel marks edges that fan out into concurrent branches;
// Condition supports data-driven (approval-gate) routing between two nodes.
public class WorkflowEdge<TState>
{
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public Func<TState, bool>? Condition { get; set; }
    public bool IsParallel { get; set; }
}

// Wraps a middleware callback: each middleware receives the current context and a
// Next delegate, so it can inspect/react around the node action (used for guardrails).
public class WorkflowMiddleware<TState>
{
    public Func<WorkflowContext<TState>, Func<Task>, Task> Execute { get; set; } = (context, next) => next();
}

// State handed to middleware during a single node hop: the workflow state plus the
// node being entered and the shared session id.
public class WorkflowContext<TState>
{
    public TState State { get; set; } = default!;
    public string NextNode { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

// The graph engine itself: owns node/edge/middleware registries and the execution
// loop that walks the graph, fans out parallel branches, respects conditional
// edges, and applies middleware around every node hop.
public class AgenticWorkflow<TState>
{
    private readonly Dictionary<string, WorkflowNode<TState>> _nodes = new();
    private readonly List<WorkflowEdge<TState>> _edges = new();
    private readonly List<WorkflowMiddleware<TState>> _middlewares = new();
    private readonly string _sessionId = Guid.NewGuid().ToString();
    private string? _initialNodeName;

    public AgenticWorkflow<TState> AddInitialNode(string name, Func<TState, Task> execute)
    {
        _initialNodeName = name;
        _nodes[name] = new WorkflowNode<TState>
        {
            Name = name,
            ExecuteAsync = execute
        };
        return this;
    }

    public AgenticWorkflow<TState> AddNode(string name, Func<TState, Task> execute)
    {
        _nodes[name] = new WorkflowNode<TState>
        {
            Name = name,
            ExecuteAsync = execute
        };
        return this;
    }

    public AgenticWorkflow<TState> AddTerminalNode(string name, Func<TState, Task>? execute = null)
    {
        _nodes[name] = new WorkflowNode<TState>
        {
            Name = name,
            ExecuteAsync = execute,
            IsTerminal = true
        };
        return this;
    }

    // Adds one parallel edge from a source to each target so the engine runs all
    // targets concurrently (e.g., Architect -> Backend + Frontend).
    public AgenticWorkflow<TState> AddParallelSplit(string source, string[] targets)
    {
        foreach (var target in targets)
        {
            _edges.Add(new WorkflowEdge<TState>
            {
                Source = source,
                Target = target,
                IsParallel = true
            });
        }
        return this;
    }

    // Adds normal edges from several sources to a single target so the engine
    // waits for every parallel branch before continuing (the synchronization join).
    public AgenticWorkflow<TState> AddParallelJoin(string[] sources, string target)
    {
        foreach (var source in sources)
        {
            _edges.Add(new WorkflowEdge<TState>
            {
                Source = source,
                Target = target
            });
        }
        return this;
    }

    // Adds a conditional edge: the runtime only travels it when Condition(state)
    // is true, enabling approval-gated routing (e.g., Reviewer -> Deployment).
    public AgenticWorkflow<TState> AddConditionalEdge(string source, string target, Func<TState, bool> condition)
    {
        _edges.Add(new WorkflowEdge<TState>
        {
            Source = source,
            Target = target,
            Condition = condition
        });
        return this;
    }

    public AgenticWorkflow<TState> AddEdge(string source, string target)
    {
        _edges.Add(new WorkflowEdge<TState>
        {
            Source = source,
            Target = target
        });
        return this;
    }

    // Registers a middleware to wrap node execution; middleware can reject/approve
    // a transition (used by the Governance guardrail).
    public AgenticWorkflow<TState> UseMiddleware(Func<WorkflowContext<TState>, Func<Task>, Task> middleware)
    {
        _middlewares.Add(new WorkflowMiddleware<TState> { Execute = middleware });
        return this;
    }

    public async Task ExecuteAsync(TState initialState)
    {
        var state = initialState;
        var currentNodeName = _initialNodeName ?? _nodes.Keys.FirstOrDefault();

        while (currentNodeName != null)
        {
            if (!_nodes.TryGetValue(currentNodeName, out var currentNode))
            {
                throw new InvalidOperationException($"Node '{currentNodeName}' not found in workflow.");
            }

            var context = new WorkflowContext<TState>
            {
                State = state,
                NextNode = currentNodeName,
                SessionId = _sessionId
            };

            using var nodeActivity = TelemetryConfiguration.ActivitySource.StartActivity($"Workflow.Node.{currentNodeName}");
            nodeActivity?.SetTag("workflow.session_id", _sessionId);
            nodeActivity?.SetTag("workflow.node_name", currentNodeName);

            if (state is CodingProjectState preState)
            {
                nodeActivity?.SetTag("workflow.goal", preState.Goal);
                if (!string.IsNullOrEmpty(preState.ArchitectureSpec)) nodeActivity?.SetTag("workflow.input_spec", preState.ArchitectureSpec);
                if (!string.IsNullOrEmpty(preState.BackendCode)) nodeActivity?.SetTag("workflow.input_backend", preState.BackendCode);
                if (!string.IsNullOrEmpty(preState.FrontendCode)) nodeActivity?.SetTag("workflow.input_frontend", preState.FrontendCode);
            }

            await ExecuteMiddlewarePipeline(context, async () =>
            {
                if (currentNode.ExecuteAsync != null)
                {
                    await currentNode.ExecuteAsync(state);
                }

                ConsoleLogger.Info($"[WORKFLOW] Executed node: {currentNodeName}");
            });

            if (state is CodingProjectState postState)
            {
                if (!string.IsNullOrEmpty(postState.ArchitectureSpec)) nodeActivity?.SetTag("workflow.output_spec", postState.ArchitectureSpec);
                if (!string.IsNullOrEmpty(postState.BackendCode)) nodeActivity?.SetTag("workflow.output_backend", postState.BackendCode);
                if (!string.IsNullOrEmpty(postState.FrontendCode)) nodeActivity?.SetTag("workflow.output_frontend", postState.FrontendCode);
                if (!string.IsNullOrEmpty(postState.ReviewNotes)) nodeActivity?.SetTag("workflow.output_review", postState.ReviewNotes);
                if (!string.IsNullOrEmpty(postState.DeploymentLogs)) nodeActivity?.SetTag("workflow.output_deployment", postState.DeploymentLogs);
            }

            if (currentNode.IsTerminal)
            {
                ConsoleLogger.Success($"[WORKFLOW] Reached terminal node: {currentNodeName}. Workflow complete.");
                break;
            }

            var outgoingEdges = _edges.Where(e => e.Source == currentNodeName).ToList();

            if (outgoingEdges.Count == 0)
            {
                ConsoleLogger.Info($"[WORKFLOW] No outgoing edges from node: {currentNodeName}. Workflow complete.");
                break;
            }

            var parallelEdges = outgoingEdges.Where(e => e.IsParallel).ToList();
            if (parallelEdges.Count > 0)
            {
                // HIGHLIGHT: The primary architecture visual. This is where the
                // structured planner/fan-out flow comes alive - a planner node fans
                // into CONCURRENT backend + frontend branches which run via
                // Task.WhenAll, then rejoin at the reviewer, before a conditional
                // (approval) edge gates deployment. Walk this block to show the DAG.
                ConsoleLogger.Info($"[WORKFLOW] Parallel split from {currentNodeName} to {string.Join(", ", parallelEdges.Select(e => e.Target))}");

                var targetCount = parallelEdges.Count;
                var currentIdx = 0;

                // Launch every parallel branch as an independent task so they can
                // truly execute concurrently, then wait for all of them below.
                var parallelTasks = parallelEdges.Select(async edge =>
                {
                    var parallelNodeName = edge.Target;
                    using var parallelActivity = TelemetryConfiguration.ActivitySource.StartActivity($"Workflow.Node.{parallelNodeName}");
                    parallelActivity?.SetTag("workflow.session_id", _sessionId);
                    parallelActivity?.SetTag("workflow.node_name", parallelNodeName);

                    if (state is CodingProjectState preParallelState)
                    {
                        parallelActivity?.SetTag("workflow.goal", preParallelState.Goal);
                        if (!string.IsNullOrEmpty(preParallelState.ArchitectureSpec)) parallelActivity?.SetTag("workflow.input_spec", preParallelState.ArchitectureSpec);
                    }

                    var branchPrefix = (Interlocked.Increment(ref currentIdx) == targetCount) ? "└──" : "├──";
                    ConsoleLogger.ParallelBranch(branchPrefix, $"Executing parallel node [{parallelNodeName}]");

                    if (_nodes.TryGetValue(parallelNodeName, out var parallelNode) && parallelNode.ExecuteAsync != null)
                    {
                        var parallelContext = new WorkflowContext<TState>
                        {
                            State = state,
                            NextNode = parallelNodeName,
                            SessionId = _sessionId
                        };

                        await ExecuteMiddlewarePipeline(parallelContext, async () =>
                        {
                            await parallelNode.ExecuteAsync(state);
                        });

                        if (state is CodingProjectState postParallelState)
                        {
                            if (!string.IsNullOrEmpty(postParallelState.BackendCode)) parallelActivity?.SetTag("workflow.output_backend", postParallelState.BackendCode);
                            if (!string.IsNullOrEmpty(postParallelState.FrontendCode)) parallelActivity?.SetTag("workflow.output_frontend", postParallelState.FrontendCode);
                        }

                        ConsoleLogger.ParallelBranch(branchPrefix, $"Completed parallel node [{parallelNodeName}]");
                    }
                });

                await Task.WhenAll(parallelTasks);

                var joinEdges = _edges.Where(e => 
                    !e.IsParallel && 
                    parallelEdges.Any(pe => pe.Target == e.Source)).ToList();

                if (joinEdges.Count > 0)
                {
                    currentNodeName = joinEdges.First().Target;
                    ConsoleLogger.Info($"[WORKFLOW] Parallel join synchronized to: {currentNodeName}");
                }
                else
                {
                    ConsoleLogger.Info("[WORKFLOW] No join node found. Workflow complete.");
                    break;
                }
            }
            else
            {
                // Conditional routing: when the outgoing edge carries an approval
                // condition, only travel it if the condition passes; otherwise the
                // graph halts (e.g., a disapproved solution never reaches deploy).
                var conditionalEdge = outgoingEdges.FirstOrDefault(e => e.Condition != null);
                if (conditionalEdge != null && conditionalEdge.Condition != null)
                {
                    if (conditionalEdge.Condition(state))
                    {
                        currentNodeName = conditionalEdge.Target;
                        ConsoleLogger.Arrow(currentNode.Name, currentNodeName);
                    }
                    else
                    {
                        ConsoleLogger.Info("[WORKFLOW] Conditional edge condition not met. Workflow complete.");
                        break;
                    }
                }
                else
                {
                    // No condition on the edge: follow the next plain outgoing edge
                    // (or stop if there is none), advancing the DAG one hop.
                    var nextEdge = outgoingEdges.FirstOrDefault(e => e.Condition == null);
                    if (nextEdge != null)
                    {
                        ConsoleLogger.Arrow(currentNode.Name, nextEdge.Target);
                        currentNodeName = nextEdge.Target;
                    }
                    else
                    {
                        currentNodeName = null;
                    }
                }
            }
        }
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
