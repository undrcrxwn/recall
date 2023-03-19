using System.Runtime.CompilerServices;

namespace Recall.Abstractions;

public interface IAsyncStateProvider
{
    public Task RetrieveAsync(IAsyncStateMachine machine, string identifier);
    public Task PreserveAsync(IAsyncStateMachine machine, string identifier);
    public Task RemoveAsync(IAsyncStateMachine machine, string identifier);
}