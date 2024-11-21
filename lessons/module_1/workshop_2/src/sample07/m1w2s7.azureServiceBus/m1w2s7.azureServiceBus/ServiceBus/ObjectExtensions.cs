using Newtonsoft.Json;

namespace m1w2s7.azureServiceBus.ServiceBus;

public static class ObjectExtensions
{
    public static string ToJson(this object @object)
        => JsonConvert.SerializeObject(@object, new JsonSerializerSettings { });
}
