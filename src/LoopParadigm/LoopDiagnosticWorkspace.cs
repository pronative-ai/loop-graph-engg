namespace AgenticWorkflowConsole.LoopParadigm;

/// <summary>
/// A dynamic code workspace designed for the Loop Engineering walkthrough.
/// It uses real-time LLM evaluation to analyze C# code modifications live,
/// providing authentic compiler diagnostics, warnings, and convergence feedback.
/// </summary>
public class LoopDiagnosticWorkspace
{
    public bool IsClean { get; private set; } = false;
    public int IterationCount { get; private set; } = 0;
    public string TargetFileName { get; } = "OrderDiscountEngine.cs";

    // HIGHLIGHT: Simulated Diagnostic Workspace - In-memory sandbox containing synthetic defects for the loop agent to discover and repair
    private string _sourceCode = """
        namespace ECommerce.Pricing;

        public class Customer
        {
            public string? Name { get; set; }
            public string? Tier { get; set; } // "Gold", "Silver", "Regular"
        }

        public class OrderDiscountEngine
        {
            public decimal CalculateDiscount(Customer customer, decimal orderTotal)
            {
                // Defect: Unresolved symbol & missing implementation
                decimal discountRate = ApplyTierDiscount(customer.Tier);

                if (orderTotal > 1000m)
                {
                    discountRate += 0.05m
                }

                return orderTotal * discountRate;
            }
        }
        """;

    public string GetSourceCode() => _sourceCode;

    /// <summary>
    /// Inspects the current source code in the workspace.
    /// </summary>
    public string InspectCode()
    {
        return $"""
            File: {TargetFileName}
            --------------------------------------------------
            {_sourceCode}
            --------------------------------------------------
            Iteration: {IterationCount} | Verified Clean: {IsClean}
            """;
    }

    /// <summary>
    /// Applies a code patch or replacement to the workspace.
    /// </summary>
    // HIGHLIGHT: Patch Application - Mutates sandbox code state and invalidates cleanliness status, forcing re-verification
    public string ApplyCodeFix(string updatedCode, string explanation)
    {
        _sourceCode = updatedCode;
        IsClean = false;
        return $"[WORKSPACE] Patch applied: {explanation}\nSource file {TargetFileName} updated. Verification required to test compiler and quality status.";
    }

    /// <summary>
    /// Compiles and verifies the current workspace code using real-time LLM analysis.
    /// </summary>
    // HIGHLIGHT: Live LLM Verification Engine - Evaluates updated C# code against compiler rules, nullable analysis, and unit tests
    public async Task<string> CompileAndVerifyAsync(IChatClient chatClient)
    {
        ArgumentNullException.ThrowIfNull(chatClient);

        IterationCount++;

        var evalPrompt = $"""
            You are the automated .NET 10 / C# 14 Roslyn Compiler and Static Analysis Engine.
            Evaluate the following C# code for compilation errors, nullable warnings, missing method implementations, and edge-case unit test coverage:

            ```csharp
            {_sourceCode}
            ```

            Evaluation Protocol:
            1. If there are syntax or compiler errors (such as missing ApplyTierDiscount method, undeclared variables, or missing semicolons):
               Output authentic MSBuild diagnostic format (e.g. error CS0103, error CS1002).
               State: "Build FAILED."
               End with: STATUS: [FAIL]
            2. If syntax is valid but there are nullable warnings (CS8602/CS8604 on customer or customer.Tier) or missing null-checks:
               Output compiler warnings (e.g. warning CS8602) and unit test failure for null inputs.
               State: "Build succeeded with warnings."
               End with: STATUS: [WARNING]
            3. If the code is fully implemented, syntactically clean, robustly null-safe (handles null customer and null tier with fallbacks), and correctly calculates discounts:
               Output:
               "Build succeeded.
                   0 Warning(s)
                   0 Error(s)
               All 4 unit tests passed (100% coverage)."
               End with: STATUS: [PASS - VERIFIED]

            Be concise, direct, and structured like MSBuild compiler output.
            """;

        try
        {
            var response = await chatClient.GetResponseAsync(evalPrompt);
            var result = response?.Text ?? "Compiler Engine: No output received.";

            if (result.Contains("STATUS: [PASS - VERIFIED]", StringComparison.OrdinalIgnoreCase) ||
                (result.Contains("0 Error(s)", StringComparison.OrdinalIgnoreCase) && 
                 result.Contains("0 Warning(s)", StringComparison.OrdinalIgnoreCase) && 
                 !result.Contains("[FAIL]", StringComparison.OrdinalIgnoreCase)))
            {
                IsClean = true;
            }
            else
            {
                IsClean = false;
            }

            return result;
        }
        catch (Exception ex)
        {
            return $"[COMPILER ENGINE ERROR] Verification failed to execute: {ex.Message}";
        }
    }
}
