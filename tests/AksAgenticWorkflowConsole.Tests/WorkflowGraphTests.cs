using System.Collections.Concurrent;
using AgenticWorkflowConsole;
using AgenticWorkflowConsole.LoopParadigm;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.InProc;
using Xunit;

namespace AksAgenticWorkflowConsole.Tests;

public class WorkflowGraphTests
{
    [Fact]
    public async Task LinearWorkflow_ExecutesAllNodesInOrder()
    {
        var workflow = new AgenticWorkflow<CodingProjectState>();
        var executed = new List<string>();

        workflow.AddInitialNode("Node1", state =>
        {
            executed.Add("Node1");
            return Task.CompletedTask;
        });

        workflow.AddEdge("Node1", "Node2");

        workflow.AddNode("Node2", state =>
        {
            executed.Add("Node2");
            return Task.CompletedTask;
        });

        workflow.AddEdge("Node2", "Node3");

        workflow.AddTerminalNode("Node3", state =>
        {
            executed.Add("Node3");
            return Task.CompletedTask;
        });

        var state = new CodingProjectState { Goal = "Test Goal" };
        await workflow.ExecuteAsync(state);

        Assert.Equal(new[] { "Node1", "Node2", "Node3" }, executed);
    }

    [Fact]
    public async Task ParallelWorkflow_ExecutesSplitAndJoin()
    {
        var workflow = new AgenticWorkflow<CodingProjectState>();
        var executed = new ConcurrentBag<string>();

        workflow.AddInitialNode("Start", state =>
        {
            executed.Add("Start");
            return Task.CompletedTask;
        });

        workflow.AddParallelSplit("Start", ["BranchA", "BranchB"]);

        workflow.AddNode("BranchA", state =>
        {
            executed.Add("BranchA");
            return Task.CompletedTask;
        });

        workflow.AddNode("BranchB", state =>
        {
            executed.Add("BranchB");
            return Task.CompletedTask;
        });

        workflow.AddParallelJoin(["BranchA", "BranchB"], "JoinNode");

        workflow.AddTerminalNode("JoinNode", state =>
        {
            executed.Add("JoinNode");
            return Task.CompletedTask;
        });

        var state = new CodingProjectState();
        await workflow.ExecuteAsync(state);

        Assert.Contains("Start", executed);
        Assert.Contains("BranchA", executed);
        Assert.Contains("BranchB", executed);
        Assert.Contains("JoinNode", executed);
    }

    [Fact]
    public async Task ConditionalEdge_RoutesBasedOnState()
    {
        var workflow = new AgenticWorkflow<CodingProjectState>();
        var executed = new List<string>();

        workflow.AddInitialNode("Reviewer", state =>
        {
            state.IsApproved = true;
            executed.Add("Reviewer");
            return Task.CompletedTask;
        });

        workflow.AddConditionalEdge("Reviewer", "Deploy", state => state.IsApproved);
        workflow.AddTerminalNode("Deploy", state =>
        {
            executed.Add("Deploy");
            return Task.CompletedTask;
        });

        var state = new CodingProjectState();
        await workflow.ExecuteAsync(state);

        Assert.Equal(new[] { "Reviewer", "Deploy" }, executed);
    }

    [Fact]
    public async Task MiddlewarePipeline_InterceptsNodeExecution()
    {
        var workflow = new AgenticWorkflow<CodingProjectState>();
        var intercepted = new List<string>();

        workflow.UseMiddleware(async (context, next) =>
        {
            intercepted.Add(context.NextNode);
            await next();
        });

        workflow.AddInitialNode("Start", _ => Task.CompletedTask);
        workflow.AddEdge("Start", "End");
        workflow.AddTerminalNode("End", _ => Task.CompletedTask);

        await workflow.ExecuteAsync(new CodingProjectState());

        Assert.Contains("Start", intercepted);
        Assert.Contains("End", intercepted);
    }

    [Fact]
    public void BuildMafWorkflow_ProducesValidMafWorkflowInstance()
    {
        var workflow = new AgenticWorkflow<CodingProjectState>();
        workflow.AddInitialNode("Start", _ => Task.CompletedTask);
        workflow.AddEdge("Start", "Finish");
        workflow.AddTerminalNode("Finish", _ => Task.CompletedTask);

        var mafWorkflow = workflow.BuildMafWorkflow();

        Assert.NotNull(mafWorkflow);
    }

    [Fact]
    public async Task MafCyclicLoopWorkflow_ExecutesIterativeCyclesUntilClean()
    {
        var workspace = new LoopDiagnosticWorkspace();
        int iterationCalls = 0;

        var stepExecutor = new FunctionExecutor<LoopDiagnosticWorkspace, LoopDiagnosticWorkspace>(
            "StepAgent",
            (ws, ctx, ct) =>
            {
                iterationCalls++;
                ws.IncrementIteration();
                if (iterationCalls >= 2)
                {
                    ws.ApplyCodeFix("clean code", "Fixing issues");
                    ws.SetCleanStatus(true);
                }
                return ValueTask.FromResult(ws);
            });

        var evalExecutor = new FunctionExecutor<LoopDiagnosticWorkspace, LoopDiagnosticWorkspace>(
            "StepEval",
            (ws, ctx, ct) => ValueTask.FromResult(ws));

        var endExecutor = new FunctionExecutor<LoopDiagnosticWorkspace, LoopDiagnosticWorkspace>(
            "StepEnd",
            (ws, ctx, ct) => ValueTask.FromResult(ws));

        var builder = new WorkflowBuilder(stepExecutor);
        builder.AddEdge(stepExecutor, evalExecutor);
        builder.AddEdge<LoopDiagnosticWorkspace>(evalExecutor, stepExecutor, ws => ws != null && !ws.IsClean && ws.IterationCount < 5);
        builder.AddEdge<LoopDiagnosticWorkspace>(evalExecutor, endExecutor, ws => ws != null && (ws.IsClean || ws.IterationCount >= 5));
        builder.WithOutputFrom(endExecutor);

        var loopWorkflow = builder.Build();
        await using var run = await InProcessExecution.RunStreamingAsync(loopWorkflow, workspace);

        await foreach (var evt in run.WatchStreamAsync())
        {
            if (evt is WorkflowOutputEvent)
            {
                break;
            }
        }

        Assert.True(iterationCalls >= 2);
        Assert.True(workspace.IsClean);
    }
}
