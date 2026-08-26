namespace AgenticWorkflowConsole;

public class CodingProjectState
{
    public string Goal { get; set; } = string.Empty;
    public bool TasksCreated { get; set; }
    public string ArchitectureSpec { get; set; } = string.Empty;
    public string BackendCode { get; set; } = string.Empty;
    public string FrontendCode { get; set; } = string.Empty;
    public string ReviewNotes { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string DeploymentLogs { get; set; } = string.Empty;
    public ConcurrentDictionary<string, string> Metadata { get; } = new();
}
