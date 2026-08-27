using AgenticWorkflowConsole;
using Xunit;

namespace AksAgenticWorkflowConsole.Tests;

public class StateAndToolTests
{
    [Fact]
    public void CodingProjectState_MaintainsProperties()
    {
        var state = new CodingProjectState
        {
            Goal = "Build walkthrough",
            TasksCreated = true,
            ArchitectureSpec = "Spec v1",
            BackendCode = "public class C {}",
            FrontendCode = "<div/>",
            ReviewNotes = "Looks good",
            IsApproved = true,
            DeploymentLogs = "Deployed"
        };

        state.Metadata["env"] = "staging";

        Assert.Equal("Build walkthrough", state.Goal);
        Assert.True(state.TasksCreated);
        Assert.True(state.IsApproved);
        Assert.Equal("staging", state.Metadata["env"]);
    }

    [Fact]
    public async Task HumanCheckpointStore_ApproveAndReject_WorkCorrectly()
    {
        var session1 = "test-session-1";
        await HumanCheckpointStore.TriggerApprovalPrompt(session1);
        HumanCheckpointStore.Approve(session1);
        var approved = await HumanCheckpointStore.WaitForApprovalAsync(session1);
        Assert.True(approved);

        var session2 = "test-session-2";
        await HumanCheckpointStore.TriggerApprovalPrompt(session2);
        HumanCheckpointStore.Reject(session2);
        var rejected = await HumanCheckpointStore.WaitForApprovalAsync(session2);
        Assert.False(rejected);
    }

    [Fact]
    public async Task TerminalExecutionTool_ExecutesSimpleCommand()
    {
        var tool = new TerminalExecutionTool();
        var command = OperatingSystem.IsWindows() ? "echo Hello" : "echo Hello";
        var result = await tool.ExecuteAsync(command);

        Assert.True(result.Success);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Hello", result.Output);
    }
}
