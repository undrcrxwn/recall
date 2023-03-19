using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Recall.Abstractions;
using Recall.Core.Extensions;
using Recall.Json.Models;

namespace Recall.Json.Services;

public class JsonAsyncStateProvider : IAsyncStateProvider
{
    public string RootDirectory { get; init; } = "memories";
    
    public async Task RetrieveAsync(IAsyncStateMachine machine, string identifier)
    {
        var path = GetFilePath(machine, identifier);

        if (!File.Exists(path))
            return;

        var json = await File.ReadAllTextAsync(path);
        var descriptors = JsonConvert.DeserializeObject<IEnumerable<FieldDescriptor>>(json)!;

        var machineType = machine.GetType();
        foreach (var descriptor in descriptors)
        {
            var field = machineType.GetField(descriptor.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field is null)
                continue;

            var value = descriptor.Value.ToObject(field.FieldType);
            field.SetValue(machine, value);
        }
    }

    public async Task PreserveAsync(IAsyncStateMachine machine, string identifier)
    {
        Console.WriteLine("Preserving state...");
        
        var descriptors = machine.GetPersistedFields()
            .Select(field =>
            {
                var success = false;
                JToken? value = null;

                try
                {
                    value = JToken.FromObject(field.GetValue(machine)!);
                    success = true;
                }
                catch { }

                return new
                {
                    SerializationSuccess = success,
                    FieldName = field.Name,
                    FieldValue = value
                };
            })
            .Where(result => result.SerializationSuccess)
            .Select(result => new FieldDescriptor(result.FieldName, result.FieldValue!));

        var json = JsonConvert.SerializeObject(descriptors);
        var path = GetFilePath(machine, identifier);
        await File.WriteAllTextAsync(path, json);
    }

    public Task RemoveAsync(IAsyncStateMachine machine, string identifier)
    {
        var path = GetFilePath(machine, identifier);
        File.Delete(path);
        return Task.CompletedTask;
    }

    private string GetFilePath(IAsyncStateMachine machine, string identifier)
    {
        var keyBytes = Encoding.UTF8.GetBytes($"{machine}:{identifier}");
        var fileName = Convert.ToBase64String(keyBytes);
        return $"{RootDirectory}/{fileName}.json";
    }
}