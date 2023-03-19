namespace Recall.Abstractions;

public interface IPersistentAsyncInvoker
{
    public Task InvokeAsync(Func<Task> job);
}