using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlatMate.Module.Offers.Domain.Rewe.Jso
{
    public class OfferJso
    {
        public Dictionary<string, string> AdditionalFields { get; set; }

        public string AdditionalInformation { get; set; }

        public string BasePrice { get; set; }

        public string Brand { get; set; }

        public string[] CategoryIDs { get; set; }

        public string Currency { get; set; }

        public string Discount { get; set; }

        public string Id { get; set; }

        [JsonProperty("_links")]
        public LinksJso Links { get; set; }

        public string Name { get; set; }

        public string Nan { get; set; }

        public OfferDurationJso OfferDuration { get; set; }

        public double Price { get; set; }

        public string ProductId { get; set; }

        public string QuantityAndUnit { get; set; }
    }
}