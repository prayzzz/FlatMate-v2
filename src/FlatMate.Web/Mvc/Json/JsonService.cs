using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;

namespace FlatMate.Web.Mvc.Json
{
    public interface IJsonService
    {
        JsonSerializerSettings ViewSerializerSettings { get; }
    }

    [Inject(DependencyLifetime.Singleton)]
    public class JsonService : IJsonService
    {
        public JsonService()
        {
            ViewSerializerSettings = new JsonSerializerSettings();
            ViewSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public JsonSerializerSettings ViewSerializerSettings { get; }
    }
}