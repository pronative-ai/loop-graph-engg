namespace AgenticWorkflowConsole;

// In-memory store for human checkpoint approvals. It tracks pending approval
// requests by session id and records the operator's approved/rejected result so a
// workflow can pause (awaiting input) and later recall the decision without
// re-prompting the user. The graph's middleware layer calls Approve/Reject
// once the operator types their verdict.
public static class HumanCheckpointStore
{
    private static readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingApprovals = new();
    private static readonly ConcurrentDictionary<string, bool> _history = new();

    public static Task TriggerApprovalPrompt(string sessionId)
    {
        _pendingApprovals.TryAdd(sessionId, new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));
        ConsoleLogger.Info($"[CHECKPOINT] Human approval initialized for session: {sessionId}");
        return Task.CompletedTask;
    }

    public static async Task<bool> WaitForApprovalAsync(string sessionId)
    {
        if (_history.TryGetValue(sessionId, out var recordedResult))
        {
            return recordedResult;
        }

        if (_pendingApprovals.TryGetValue(sessionId, out var tcs))
        {
            var result = await tcs.Task;
            _history[sessionId] = result;
            return result;
        }

        ConsoleLogger.Info($"[CHECKPOINT] No pending approval found for session: {sessionId}. Defaulting to true.");
        return true;
    }

    public static void Approve(string sessionId)
    {
        _history[sessionId] = true;
        if (_pendingApprovals.TryRemove(sessionId, out var tcs))
        {
            ConsoleLogger.Success($"[CHECKPOINT] Operator APPROVED session: {sessionId}");
            tcs.TrySetResult(true);
        }
        else
        {
            ConsoleLogger.Info($"[CHECKPOINT] Operator APPROVED session: {sessionId}");
        }
    }

    public static void Reject(string sessionId)
    {
        _history[sessionId] = false;
        if (_pendingApprovals.TryRemove(sessionId, out var tcs))
        {
            ConsoleLogger.SecurityWarning($"[CHECKPOINT] Operator REJECTED session: {sessionId}");
            tcs.TrySetResult(false);
        }
        else
        {
            ConsoleLogger.SecurityWarning($"[CHECKPOINT] Operator REJECTED session: {sessionId}");
        }
    }

    public static void Clear()
    {
        _pendingApprovals.Clear();
        _history.Clear();
    }
}
