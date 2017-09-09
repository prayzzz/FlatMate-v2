using Newtonsoft.Json;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;

namespace FlatMate.Web.Mvc.Json
{
    public interface IJsonService
    {
        JsonSerializerSettings SerializerSettings { get; }

        T Deserialize<T>(string obj);

        string Serialize(object result);
    }

    [Inject(DependencyLifetime.Singleton)]
    public class JsonService : IJsonService
    {
        public JsonService()
        {
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.ContractResolver = FlatMateContractResolver.Instance;
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public JsonSerializerSettings SerializerSettings { get; }

        public T Deserialize<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj, SerializerSettings);
        }

        public string Serialize(object result)
        {
            return JsonConvert.SerializeObject(result, SerializerSettings);
        }
    }
}