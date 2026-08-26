using System.Collections.Concurrent;
using AgenticWorkflowConsole;
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
}
