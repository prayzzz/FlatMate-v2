using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlatMate.Module.Offers.Domain.Import.Penny.Jso
{
    public class Envelope
    {
        [JsonProperty("Themenwelten")]
        public List<CategoryJso> Categories { get; set; } = new List<CategoryJso>();

        [JsonProperty("Angebote")]
        public List<OfferJso> Offers { get; set; } = new List<OfferJso>();
    }
}