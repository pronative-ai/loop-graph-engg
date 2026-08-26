namespace AgenticWorkflowConsole;

public class TerminalExecutionTool
{
    public async Task<TerminalExecutionResult> ExecuteAsync(
        string command,
        string? workingDirectory = null)
    {
        var result = new TerminalExecutionResult();
        
        try
        {
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

            await process.WaitForExitAsync();

            result.Output = outputBuilder.ToString().Trim();
            result.Error = errorBuilder.ToString().Trim();
            result.ExitCode = process.ExitCode;
            result.Success = process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            result.Error = $"Failed to execute command: {ex.Message}";
            result.Success = false;
            result.ExitCode = -1;
        }

        return result;
    }

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
