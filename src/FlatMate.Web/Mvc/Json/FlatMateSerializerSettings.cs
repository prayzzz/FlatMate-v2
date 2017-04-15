using Newtonsoft.Json;

namespace FlatMate.Web.Mvc.Json
{
    public class FlatMateSerializerSettings : JsonSerializerSettings
    {
        private static FlatMateSerializerSettings _instance;

        private FlatMateSerializerSettings()
        {
            Apply(this);
        }

        public static JsonSerializerSettings Instance => _instance ?? (_instance = new FlatMateSerializerSettings());

        public static void Apply(JsonSerializerSettings settings)
        {
            settings.ContractResolver = FlatMateContractResolver.Instance;
            settings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}