namespace AgenticWorkflowConsole;

public static class HumanCheckpointStore
{
    private static readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingApprovals = new();

    public static Task TriggerApprovalPrompt(string sessionId)
    {
        var tcs = new TaskCompletionSource<bool>();
        _pendingApprovals[sessionId] = tcs;

        Console.WriteLine($"[CHECKPOINT] Approval required for session: {sessionId}");
        Console.WriteLine($"[CHECKPOINT] Use HumanCheckpointStore.Approve('{sessionId}') or Reject('{sessionId}') to proceed.");

        return Task.CompletedTask;
    }

    public static async Task<bool> WaitForApprovalAsync(string sessionId)
    {
        if (_pendingApprovals.TryGetValue(sessionId, out var tcs))
        {
            return await tcs.Task;
        }

        Console.WriteLine($"[CHECKPOINT] No pending approval found for session: {sessionId}. Assuming approved.");
        return true;
    }

    public static void Approve(string sessionId)
    {
        if (_pendingApprovals.TryRemove(sessionId, out var tcs))
        {
            Console.WriteLine($"[CHECKPOINT] Approved session: {sessionId}");
            tcs.SetResult(true);
        }
        else
        {
            Console.WriteLine($"[CHECKPOINT] No pending approval found for session: {sessionId}");
        }
    }

    public static void Reject(string sessionId)
    {
        if (_pendingApprovals.TryRemove(sessionId, out var tcs))
        {
            Console.WriteLine($"[CHECKPOINT] Rejected session: {sessionId}");
            tcs.SetResult(false);
        }
        else
        {
            Console.WriteLine($"[CHECKPOINT] No pending approval found for session: {sessionId}");
        }
    }

    public static void Clear()
    {
        _pendingApprovals.Clear();
    }
}
