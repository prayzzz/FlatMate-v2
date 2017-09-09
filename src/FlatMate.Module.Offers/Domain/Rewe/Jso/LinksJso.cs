using Newtonsoft.Json;

namespace FlatMate.Module.Offers.Domain.Rewe.Jso
{
    public class LinksJso
    {
        [JsonProperty("image:digital")]
        public ImageLinkJso ImageDigital { get; set; }

        [JsonProperty("image:m")]
        public ImageLinkJso ImageM { get; set; }

        [JsonProperty("image:xl")]
        public ImageLinkJso ImageXl { get; set; }
    }
}
