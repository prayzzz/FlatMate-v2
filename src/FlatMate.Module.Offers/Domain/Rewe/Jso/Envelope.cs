using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlatMate.Module.Offers.Domain.Rewe.Jso
{
    public class Envelope<T>
    {
        public List<T> Items { get; set; }

        [JsonProperty("_meta")]
        public Dictionary<string, object> Meta { get; set; }

        public PagingJso Paging { get; set; }
    }
}