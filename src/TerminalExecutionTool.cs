namespace AgenticWorkflowConsole;

// A thin wrapper around a real subprocess call. It lets an agent (or walkthrough) run an
// external command - e.g. `dotnet build` - capture stdout/stderr and the exit
// code without spawning a visible console window. This is the "live external tool"
// used by the Loop walkthrough to observe the real compiler state and by RunBuildVerificationAsync.
public class TerminalExecutionTool
{
    // HIGHLIGHT: The external tool-invocation method. Runs a shell command as a
    // child process, asynchronously collecting its output and exit code so the
    // caller can react to what actually happened on the machine.
    public async Task<TerminalExecutionResult> ExecuteAsync(
        string command,
        string? workingDirectory = null)
    {
        var result = new TerminalExecutionResult();
        
        try
        {
            // Build a shell process that runs the command with output redirected
            // (no window, no interactive shell) so we can capture the results.
            var processStartInfo = new ProcessStartInfo
            {
                FileName = GetShellFileName(),
                Arguments = GetShellArguments(command),
                WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            
            // Accumulate stdout/stderr incrementally as the process streams them,
            // so long-running builds don't buffer until completion.
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait asynchronously (no thread-blocking) for the process to finish.
            await process.WaitForExitAsync();

            // Summarize the run for the caller from exit code + captured streams.
            result.Output = outputBuilder.ToString().Trim();
            result.Error = errorBuilder.ToString().Trim();
            result.ExitCode = process.ExitCode;
            result.Success = process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            // Any spawn/runtime failure is reported as a failed result rather than
            // thrown, so callers always get a well-formed TerminalExecutionResult.
            result.Error = $"Failed to execute command: {ex.Message}";
            result.Success = false;
            result.ExitCode = -1;
        }

        return result;
    }

    // High-level helper: locates the nearest .csproj and runs `dotnet build`,
    // producing a human-readable pass/fail summary for the agent to consume.
    public async Task<string> RunBuildVerificationAsync(string? targetPath = null)
    {
        var baseDir = targetPath ?? AppContext.BaseDirectory;
        // Locate closest csproj or directory
        var csproj = Directory.GetFiles(baseDir, "*.csproj", SearchOption.AllDirectories).FirstOrDefault()
            ?? Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj", SearchOption.AllDirectories).FirstOrDefault();

        var command = csproj != null
            ? $"dotnet build \"{csproj}\" --nologo -v q"
            : "dotnet build --nologo -v q";

        var result = await ExecuteAsync(command);
        if (result.Success)
        {
            return $"Build Succeeded. All diagnostics clean (Exit code: 0).\n{result.Output}";
        }

        return $"Build Failed (Exit code: {result.ExitCode}).\nErrors:\n{result.Output}\n{result.Error}";
    }

    // Pick the correct shell for the current OS (cmd on Windows, bash elsewhere).
    private static string GetShellFileName()
    {
        if (OperatingSystem.IsWindows())
        {
            return "cmd.exe";
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            return "/bin/bash";
        }
        else
        {
            return "/bin/sh";
        }
    }

    // Format the command for the target shell: `/c` on cmd, `-c` on POSIX shells.
    private static string GetShellArguments(string command)
    {
        if (OperatingSystem.IsWindows())
        {
            return $"/c {command}";
        }
        else
        {
            return $"-c \"{command.Replace("\"", "\\\"")}\"";
        }
    }
}

// Immutable-ish result object summarizing a completed command execution. Summary is
// a single ready-to-log string derived from the captured output/error/exit code.
public class TerminalExecutionResult
{
    public string Output { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public bool Success { get; set; }

    public string Summary => Success
        ? $"Command completed successfully (exit code: {ExitCode})\n{Output}"
        : $"Command failed (exit code: {ExitCode}).\nError: {Error}\nOutput: {Output}";
}
