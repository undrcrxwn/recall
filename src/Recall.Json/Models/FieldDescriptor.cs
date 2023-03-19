using Newtonsoft.Json.Linq;

namespace Recall.Json.Models;

public sealed record FieldDescriptor(string Name, JToken Value);