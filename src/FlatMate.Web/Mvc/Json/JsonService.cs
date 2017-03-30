using Newtonsoft.Json;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;

namespace FlatMate.Web.Mvc.Json
{
    public interface IJsonService
    {
        JsonSerializerSettings SerializerSettings { get; }

        string Serialize(object result);

        T Deserialize<T>(string obj);
    }

    [Inject(DependencyLifetime.Singleton)]
    public class JsonService : IJsonService
    {
        public JsonService()
        {
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.ContractResolver = FlatMateContractResolver.Instance;
        }

        public JsonSerializerSettings SerializerSettings { get; }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        public T Deserialize<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj, SerializerSettings);
        }
    }
}