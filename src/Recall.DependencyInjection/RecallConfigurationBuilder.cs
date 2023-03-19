using Microsoft.Extensions.DependencyInjection;
using Recall.Abstractions;

namespace Recall.DependencyInjection;

public sealed class RecallConfigurationBuilder
{
    private readonly IServiceCollection _services;
    public RecallConfigurationBuilder(IServiceCollection services) => _services = services;

    public RecallConfigurationBuilder UseProvider<TProvider>() where TProvider : class, IAsyncStateProvider
    {
        _services.AddSingleton<IAsyncStateProvider, TProvider>();
        return this;
    }

    public RecallConfigurationBuilder UseProvider(IAsyncStateProvider provider)
    {
        _services.AddSingleton(provider);
        return this;
    }
}