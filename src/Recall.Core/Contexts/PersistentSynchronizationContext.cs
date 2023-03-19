using System.Runtime.CompilerServices;
using Recall.Core.Constants;
using Recall.Core.Exceptions;

namespace Recall.Core.Contexts;

public delegate Task AsyncStateMachineHandler(IAsyncStateMachine machine);

public sealed class PersistentSynchronizationContext : SynchronizationContext
{
    public IAsyncStateMachine? Machine { get; private set; }
    public string Identifier = null!;
    private readonly TaskCompletionSource _jobCompletionSource;

    private readonly Queue<AsyncStateMachineHandler> _jobs = new();

    public void Schedule(AsyncStateMachineHandler job) => _jobs.Enqueue(job);

    public PersistentSynchronizationContext(TaskCompletionSource tcs) => _jobCompletionSource = tcs;

    public override async void Post(SendOrPostCallback callback, object? state)
    {
        Machine ??= (IAsyncStateMachine?)state!.GetType()
            .GetField(MemberNames.StateMachineField)
            ?.GetValue(state)!;

        while (_jobs.TryDequeue(out var job))
        {
            try
            {
                await job.Invoke(Machine);
            }
            catch (OperationInterruptedException exception)
            {
                _jobCompletionSource.SetException(exception);
                return;
            }
        }

        base.Post(s =>
        {
            SetSynchronizationContext(this);
            callback(s);
        }, state);
    }
}