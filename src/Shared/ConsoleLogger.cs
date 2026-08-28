namespace AgenticWorkflowConsole.Shared;

// Central console output helper: every walkthrough writes through this so the terminal
// formatting stays consistent (colors, borders, separators) and the walkthroughs stay
// free of presentation code. Also owns the timing/sleep helpers so the live
// presentation reads as a scripted walkthrough.
public static class ConsoleLogger
{
    /// <summary>
    /// Displays a prominent, large ASCII brand banner in bright green upon application startup.
    /// </summary>
    public static void BrandBanner()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("""
========================================================================================================
  ██████╗ ██████╗  ██████╗ ███╗   ██╗ █████╗ ████████╗██╗██╗   ██╗███████╗     █████╗ ██╗
  ██╔══██╗██╔══██╗██╔═══██╗████╗  ██║██╔══██╗╚══██╔══╝██║██║   ██║██╔════╝    ██╔══██╗██║
  ██████╔╝██████╔╝██║   ██║██╔██╗ ██║███████║   ██║   ██║██║   ██║█████╗      ███████║██║
  ██╔═══╝ ██╔══██╗██║   ██║██║╚██╗██║██╔══██║   ██║   ██║╚██╗ ██╔╝██╔══╝      ██╔══██║██║
  ██║     ██║  ██║╚██████╔╝██║ ╚████║██║  ██║   ██║   ██║ ╚████╔╝ ███████╗██╗██║  ██║██║
  ╚═╝     ╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═══╝  ╚══════╝╚═╝╚═╝  ╚═╝╚═╝
========================================================================================================
                 ⚡ AI AGENTIC WORKFLOWS: LOOP VS GRAPH ENGINEERING (MAF) ⚡
""");
        Console.ResetColor();
    }

    public static void SectionHeader(string title)
    {
        WriteColor($"\n>>> {title} <<<", ConsoleColor.Yellow);
    }

    public static void GraphBorder(string message)
    {
        WriteColor($"[===] {message} [===]", ConsoleColor.Magenta);
    }

    public static void LoopBorder(string message)
    {
        WriteColor($"[---] {message} [---]", ConsoleColor.Yellow);
    }

    public static void LlmReasoning(int iteration, string message)
    {
        WriteColor($"[Loop #{iteration}] [LLM REASONING] {message}", ConsoleColor.Blue);
    }

    public static void ToolCall(int iteration, string message)
    {
        WriteColor($"[Loop #{iteration}] [TOOL CALL] {message}", ConsoleColor.Cyan);
    }

    public static void Observation(int iteration, string message)
    {
        WriteColor($"[Loop #{iteration}] [OBSERVATION] {message}", ConsoleColor.Gray);
    }

    public static void Arrow(string from, string to)
    {
        WriteColor($"[{from}] ---> [{to}]", ConsoleColor.Magenta);
    }

    public static void TreeBranch(string prefix, string message, bool isLast = false)
    {
        var connector = isLast ? "└── " : "├── ";
        WriteColor($"{prefix}{connector}{message}", ConsoleColor.DarkCyan);
    }

    public static void ParallelBranch(string branchSymbol, string message)
    {
        WriteColor($"{branchSymbol} {message}", ConsoleColor.DarkCyan);
    }

    public static void SecurityWarning(string message)
    {
        var prevBg = Console.BackgroundColor;
        var prevFg = Console.ForegroundColor;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"!! [{message}] !!");
        Console.BackgroundColor = prevBg;
        Console.ForegroundColor = prevFg;
    }

    public static void Info(string message)
    {
        WriteColor(message, ConsoleColor.White);
    }

    public static void Highlight(string message)
    {
        WriteColor(message, ConsoleColor.Cyan);
    }

    public static void MenuOption(string key, string title)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"  [{key}] ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(title);
        Console.ForegroundColor = prev;
    }

    public static void StreamToken(string text, ConsoleColor color = ConsoleColor.White)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = prev;
    }

    public static void Success(string message)
    {
        WriteColor(message, ConsoleColor.Green);
    }

    public static void Pause(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    public static void BlankLine()
    {
        Console.WriteLine();
    }

    public static void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    private static void WriteColor(string message, ConsoleColor color)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = prev;
    }
}