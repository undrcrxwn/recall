using Microsoft.Extensions.DependencyInjection;
using Recall.Abstractions;
using Recall.Core.Services;

namespace Recall.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static RecallConfigurationBuilder AddRecall(this IServiceCollection services)
    {
        services
            .AddSingleton<IAsyncStateManager, AsyncStateManager>()
            .AddSingleton<IPersistentAsyncInvoker, PersistentAsyncInvoker>();
        
        return new RecallConfigurationBuilder(services);
    }
}