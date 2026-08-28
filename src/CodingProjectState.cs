namespace AgenticWorkflowConsole;

// HIGHLIGHT: Blackboard State Pattern - Central state object passed across all DAG graph nodes
// Each specialized agent reads existing context, performs work, and writes deliverables into this shared container.
public class CodingProjectState
{
    // Goal initialized by user prompt
    public string Goal { get; set; } = string.Empty;
    public bool TasksCreated { get; set; }
    
    // Architect agent deliverables
    public string ArchitectureSpec { get; set; } = string.Empty;
    
    // Coder agent deliverables
    public string BackendCode { get; set; } = string.Empty;
    public string FrontendCode { get; set; } = string.Empty;
    
    // Reviewer & Governance guardrail outputs
    public string ReviewNotes { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    
    // Runner agent deployment output
    public string DeploymentLogs { get; set; } = string.Empty;
    
    // Dynamic execution metadata (telemetry tags, checkpoint tokens, execution metrics)
    public ConcurrentDictionary<string, string> Metadata { get; } = new();
}
