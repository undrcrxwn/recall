using System.Reflection;
using System.Runtime.CompilerServices;
using Recall.Core.Constants;

namespace Recall.Core.Extensions;

public static class AsyncStateMachineExtensions
{
    public static int GetState(this IAsyncStateMachine machine) =>
        (int)machine.GetStateField().GetValue(machine)!;

    public static FieldInfo GetStateField(this IAsyncStateMachine machine) =>
        machine.GetType().GetField(MemberNames.StateField, BindingFlags.Instance | BindingFlags.Public)!;

    public static IEnumerable<FieldInfo> GetPersistedFields(this IAsyncStateMachine machine) =>
        machine.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field =>
                field.Name == MemberNames.StateField ||
                !field.Name.StartsWith(MemberNames.GeneratedPrefix));
}