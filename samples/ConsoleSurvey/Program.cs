using ConsoleSurvey;
using Microsoft.Extensions.DependencyInjection;
using Recall.Abstractions;
using Recall.DependencyInjection;
using Recall.Json.Extensions;

// Dependency injection
var services = new ServiceCollection()
    .AddSingleton<PersistentSurveyService>();

services
    .AddRecall()
    .UseJsonFileProvider();

// Build services
var provider = services.BuildServiceProvider();
var invoker = provider.GetRequiredService<IPersistentAsyncInvoker>();
var service = provider.GetRequiredService<PersistentSurveyService>();

// Client code
await invoker.InvokeAsync(service.PerformSurveyAsync);

namespace ConsoleSurvey
{
    internal class PersistentSurveyService
    {
        private readonly IAsyncStateManager _state;
        public PersistentSurveyService(IAsyncStateManager state) => _state = state;
    
        public async Task PerformSurveyAsync()
        {
            await _state.Recall();

            System.Console.WriteLine("1. What's your name?");
            var name = System.Console.ReadLine()!;
            await _state.Commit();

            System.Console.WriteLine("2. How old are you?");
            var age = int.Parse(System.Console.ReadLine()!);
            await _state.Commit();

            System.Console.WriteLine("Thanks for your time! Here are your answers:");
            System.Console.WriteLine($"1. Name: {name}");
            System.Console.WriteLine($"2. Age: {age}");
            await _state.Forget();
        }
    }
}