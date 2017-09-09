using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FlatMate.Module.Offers.Domain.Rewe.Jso
{
    public class Envelope<T>
    {
        public List<T> Items { get; set; }

        [JsonProperty("_meta")]
        public Dictionary<string, JToken> Meta { get; set; }

        public PagingJso Paging { get; set; }
    }
}
