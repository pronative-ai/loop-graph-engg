namespace AgenticWorkflowConsole;

// HIGHLIGHT: In-Memory Checkpoint Store - Tracks pending human approval requests and records operator verdicts
// Enables asynchronous human-in-the-loop workflows where execution pauses until explicit approval or rejection.
public static class HumanCheckpointStore
{
    // HIGHLIGHT: Async Approval Signals via TaskCompletionSource - Manages in-flight asynchronous wait handles unblocking human review decisions
    private static readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingApprovals = new();
    
    // HIGHLIGHT: Checkpoint Audit History - Retains session approval/rejection verdicts for idempotent query and post-execution inspection
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
