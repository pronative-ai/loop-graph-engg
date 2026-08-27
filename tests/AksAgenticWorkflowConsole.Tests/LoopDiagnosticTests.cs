using AgenticWorkflowConsole.LoopParadigm;
using Xunit;

namespace AksAgenticWorkflowConsole.Tests;

public class LoopDiagnosticTests
{
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
    public async Task CompileAndVerifyAsync_Offline_IncrementsIteration()
    {
        var workspace = new LoopDiagnosticWorkspace();
        var output = await workspace.CompileAndVerifyAsync(null);

        Assert.Contains("Offline build check", output);
        Assert.Equal(1, workspace.IterationCount);
    }
}
