using Recall.DependencyInjection;
using Recall.Json.Services;

namespace Recall.Json.Extensions;

public static class RecallConfigurationBuilderExtensions
{
    public static RecallConfigurationBuilder UseJsonFileProvider(this RecallConfigurationBuilder builder, string? rootDirectory = default)
    {
        var provider = new JsonAsyncStateProvider { RootDirectory = rootDirectory ?? "memories" };
        return builder.UseProvider(provider);
    }
}