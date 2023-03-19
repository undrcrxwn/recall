using System.Runtime.CompilerServices;
using Recall.Abstractions;
using Recall.Core.Contexts;
using Recall.Core.Exceptions;
using Recall.Core.Extensions;

namespace Recall.Core.Services;

public class AsyncStateManager : IAsyncStateManager
{
    private readonly IAsyncStateProvider _stateProvider;

    public AsyncStateManager(IAsyncStateProvider stateProvider) => _stateProvider = stateProvider;

    public YieldAwaitable Recall(string? identifier = default)
    {
        var context =
            SynchronizationContext.Current as PersistentSynchronizationContext
            ?? throw new InvalidOperationException($"{nameof(Recall)} can only be performed under {nameof(PersistentSynchronizationContext)}.");

        context.Identifier = identifier ?? string.Empty;
        context.Schedule(async machine =>
            await _stateProvider.RetrieveAsync(machine, context.Identifier));

        return Task.Yield();
    }

    public YieldAwaitable Memorize(string? identifier = default)
    {
        var context =
            SynchronizationContext.Current as PersistentSynchronizationContext
            ?? throw new InvalidOperationException($"{nameof(Memorize)} can only be performed under {nameof(PersistentSynchronizationContext)}.");

        identifier ??= context.Identifier;
        context.Schedule(async machine =>
        {
            switch (machine.GetState())
            {
                case -1:
                    return;
                case -2:
                    await _stateProvider.RemoveAsync(context.Machine!, identifier);
                    return;
                default:
                    await _stateProvider.PreserveAsync(machine, identifier);
                    return;
            }
        });

        return Task.Yield();
    }

    public YieldAwaitable Commit(string? identifier = default)
    {
        var context =
            SynchronizationContext.Current as PersistentSynchronizationContext
            ?? throw new InvalidOperationException($"{nameof(Commit)} can only be performed under {nameof(PersistentSynchronizationContext)}.");
        
        var yield = Memorize(identifier);
        context.Schedule(_ => throw new OperationInterruptedException());

        return yield;
    }

    public async Task Forget(string? identifier = default)
    {
        var context =
            SynchronizationContext.Current as PersistentSynchronizationContext
            ?? throw new InvalidOperationException($"{nameof(Forget)} can only be performed under {nameof(PersistentSynchronizationContext)}.");

        identifier ??= context.Identifier;
        await _stateProvider.RemoveAsync(context.Machine!, identifier);
    }
}