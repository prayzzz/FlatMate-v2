using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlatMate.Module.Offers.Domain.Adapter.Penny
{
    public class Envelope
    {
        [JsonProperty("Angebote")]
        public List<OfferJso> Offers { get; set; } = new List<OfferJso>();

        [JsonProperty("Themenwelten")]
        public List<CategoryJso> Categories { get; set; } = new List<CategoryJso>();
    }
}
