using Recall.Abstractions;
using Recall.Core.Contexts;

namespace Recall.Core.Services;

public class PersistentAsyncInvoker : IPersistentAsyncInvoker
{
    public async Task InvokeAsync(Func<Task> job)
    {
        var tcs = new TaskCompletionSource();
        var context = new PersistentSynchronizationContext(tcs);
        SynchronizationContext.SetSynchronizationContext(context);

        var magicTask = job();
        await Task.WhenAny(magicTask, tcs.Task);

        if (magicTask.Exception is not null)
            throw magicTask.Exception;
    }
}