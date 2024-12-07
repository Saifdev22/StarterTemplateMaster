using Newtonsoft.Json;

namespace Common.Infrastructure.Serialization;

public static class SerializerSettings
{
    public static readonly JsonSerializerSettings Instance = new()
    {
#pragma warning disable CA2326 // Do not use TypeNameHandling values other than None
        TypeNameHandling = TypeNameHandling.All,
#pragma warning restore CA2326 // Do not use TypeNameHandling values other than None
        MetadataPropertyHandling = MetadataPropertyHandling.Default
    };
}
