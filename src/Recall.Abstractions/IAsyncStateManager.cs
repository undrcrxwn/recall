using System.Runtime.CompilerServices;

namespace Recall.Abstractions;

public interface IAsyncStateManager
{
    public YieldAwaitable Recall(string? identifier = default);
    public YieldAwaitable Memorize(string? identifier = default);
    public YieldAwaitable Commit(string? identifier = default);
    public Task Forget(string? identifier = default);
}