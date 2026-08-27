namespace AgenticWorkflowConsole;

// Shared state model threaded through the entire graph: each node reads/writes a
// CodingProjectState as it progresses from goal definition to deployment, so the
// graph's nodes communicate by mutating this bag rather than passing parameters.
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
