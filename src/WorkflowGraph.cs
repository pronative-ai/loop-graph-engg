using System.Collections.Concurrent;

namespace AksAgenticWorkflowConsole;

public class WorkflowNode<TState>
{
    public string Name { get; set; } = string.Empty;
    public Func<TState, Task>? ExecuteAsync { get; set; }
    public bool IsTerminal { get; set; }
}

public class WorkflowEdge<TState>
{
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public Func<TState, bool>? Condition { get; set; }
    public bool IsParallel { get; set; }
}

public class WorkflowMiddleware<TState>
{
    public Func<WorkflowContext<TState>, Func<Task>, Task> Execute { get; set; } = (context, next) => next();
}

public class WorkflowContext<TState>
{
    public TState State { get; set; } = default!;
    public string NextNode { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

public class AgenticWorkflow<TState>
{
    private readonly Dictionary<string, WorkflowNode<TState>> _nodes = new();
    private readonly List<WorkflowEdge<TState>> _edges = new();
    private readonly List<WorkflowMiddleware<TState>> _middlewares = new();
    private readonly string _sessionId = Guid.NewGuid().ToString();

    public AgenticWorkflow<TState> AddInitialNode(string name, Func<TState, Task> execute)
    {
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

    public AgenticWorkflow<TState> UseMiddleware(Func<WorkflowContext<TState>, Func<Task>, Task> middleware)
    {
        _middlewares.Add(new WorkflowMiddleware<TState> { Execute = middleware });
        return this;
    }

    public async Task ExecuteAsync(TState initialState)
    {
        var state = initialState;
        var currentNodeName = _edges.FirstOrDefault(e => e.IsParallel)?.Source ?? _nodes.Keys.First();
        var visitedNodes = new HashSet<string>();
        var pendingNodes = new ConcurrentBag<string>();

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

            await ExecuteMiddlewarePipeline(context, async () =>
            {
                if (currentNode.ExecuteAsync != null)
                {
                    await currentNode.ExecuteAsync(state);
                }

                Console.WriteLine($"[WORKFLOW] Executed node: {currentNodeName}");
            });

            visitedNodes.Add(currentNodeName);

            if (currentNode.IsTerminal)
            {
                Console.WriteLine($"[WORKFLOW] Reached terminal node: {currentNodeName}. Workflow complete.");
                break;
            }

            var outgoingEdges = _edges.Where(e => e.Source == currentNodeName).ToList();

            if (outgoingEdges.Count == 0)
            {
                Console.WriteLine($"[WORKFLOW] No outgoing edges from node: {currentNodeName}. Workflow complete.");
                break;
            }

            var parallelEdges = outgoingEdges.Where(e => e.IsParallel).ToList();
            if (parallelEdges.Count > 0)
            {
                Console.WriteLine($"[WORKFLOW] Parallel split from {currentNodeName} to {string.Join(", ", parallelEdges.Select(e => e.Target))}");
                
                var parallelTasks = parallelEdges.Select(async edge =>
                {
                    var parallelNodeName = edge.Target;
                    if (_nodes.TryGetValue(parallelNodeName, out var parallelNode) && parallelNode.ExecuteAsync != null)
                    {
                        await parallelNode.ExecuteAsync(state);
                        Console.WriteLine($"[WORKFLOW] Completed parallel node: {parallelNodeName}");
                        pendingNodes.Add(parallelNodeName);
                    }
                });

                await Task.WhenAll(parallelTasks);

                var joinEdges = _edges.Where(e => 
                    !e.IsParallel && 
                    parallelEdges.Any(pe => pe.Target == e.Source)).ToList();

                if (joinEdges.Count > 0)
                {
                    currentNodeName = joinEdges.First().Target;
                    Console.WriteLine($"[WORKFLOW] Parallel join to: {currentNodeName}");
                }
                else
                {
                    Console.WriteLine($"[WORKFLOW] No join node found. Workflow complete.");
                    break;
                }
            }
            else
            {
                var conditionalEdge = outgoingEdges.FirstOrDefault(e => e.Condition != null);
                if (conditionalEdge != null && conditionalEdge.Condition != null)
                {
                    if (conditionalEdge.Condition(state))
                    {
                        currentNodeName = conditionalEdge.Target;
                        Console.WriteLine($"[WORKFLOW] Conditional edge to: {currentNodeName}");
                    }
                    else
                    {
                        Console.WriteLine($"[WORKFLOW] Conditional edge condition not met. Workflow complete.");
                        break;
                    }
                }
                else
                {
                    var nextEdge = outgoingEdges.FirstOrDefault(e => e.Condition == null);
                    currentNodeName = nextEdge?.Target;
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
