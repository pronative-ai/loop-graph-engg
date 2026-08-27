using AgenticWorkflowConsole.LoopParadigm;
using Microsoft.Extensions.AI;
using Xunit;

namespace AksAgenticWorkflowConsole.Tests;

public class LoopDiagnosticTests
{
    private sealed class MockEvaluatorChatClient(Func<string, string> responseFactory) : IChatClient
    {
        public ChatClientMetadata Metadata => new("MockEvaluator");

        public Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> chatMessages, 
            ChatOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            var userText = chatMessages.LastOrDefault()?.Text ?? string.Empty;
            var reply = responseFactory(userText);
            return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, reply)));
        }

        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> chatMessages, 
            ChatOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public object? GetService(Type serviceType, object? serviceKey = null) => null;
        public void Dispose() { }
    }

    [Fact]
    public void InitialState_And_CodeInspection_Work()
    {
        var workspace = new LoopDiagnosticWorkspace();
        Assert.False(workspace.IsClean);
        Assert.Equal(0, workspace.IterationCount);
        Assert.Equal("OrderDiscountEngine.cs", workspace.TargetFileName);

        var inspect = workspace.InspectCode();
        Assert.Contains("OrderDiscountEngine.cs", inspect);
        Assert.Contains("ApplyTierDiscount", inspect);
    }

    [Fact]
    public void ApplyCodeFix_UpdatesSourceAndResetsCleanFlag()
    {
        var workspace = new LoopDiagnosticWorkspace();
        var patch = "// refined code implementation";
        var result = workspace.ApplyCodeFix(patch, "Implemented tier discounts");

        Assert.Contains("Patch applied", result);
        Assert.Equal(patch, workspace.GetSourceCode());
        Assert.False(workspace.IsClean);
    }

    [Fact]
    public async Task CompileAndVerifyAsync_NullChatClient_ThrowsArgumentNullException()
    {
        var workspace = new LoopDiagnosticWorkspace();
        await Assert.ThrowsAsync<ArgumentNullException>(() => workspace.CompileAndVerifyAsync(null!));
    }

    [Fact]
    public async Task CompileAndVerifyAsync_WithChatClient_EvaluatesCodeAndUpdatesState()
    {
        var workspace = new LoopDiagnosticWorkspace();
        var mockClient = new MockEvaluatorChatClient(prompt =>
            "Build FAILED.\nOrderDiscountEngine.cs(16,36): error CS0103\nSTATUS: [FAIL]");

        var output = await workspace.CompileAndVerifyAsync(mockClient);

        Assert.Contains("Build FAILED", output);
        Assert.Contains("STATUS: [FAIL]", output);
        Assert.Equal(1, workspace.IterationCount);
        Assert.False(workspace.IsClean);

        // Next evaluation with clean pass
        var mockCleanClient = new MockEvaluatorChatClient(prompt =>
            "Build succeeded.\n0 Warning(s)\n0 Error(s)\nSTATUS: [PASS - VERIFIED]");

        var cleanOutput = await workspace.CompileAndVerifyAsync(mockCleanClient);

        Assert.Contains("STATUS: [PASS - VERIFIED]", cleanOutput);
        Assert.Equal(2, workspace.IterationCount);
        Assert.True(workspace.IsClean);
    }
}
