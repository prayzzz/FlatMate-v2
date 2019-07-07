using Newtonsoft.Json;

namespace FlatMate.Module.Offers.Domain.Import.Penny.Jso
{
    public class OfferJso
    {
        public string Beschreibung { get; set; }

        public string Bild { get; set; }

        public string Bilder { get; set; }

        [JsonProperty("Bild_full")]
        public string BildFull { get; set; }

        [JsonProperty("Bild_high")]
        public string BildHigh { get; set; }

        [JsonProperty("Bild_normal")]
        public string BildNormal { get; set; }

        [JsonProperty("Bild_original")]
        public string BildOriginal { get; set; }

        [JsonProperty("Bild_preview")]
        public string BildPreview { get; set; }

        [JsonProperty("Button_link")]
        public string ButtonLink { get; set; }

        [JsonProperty("Button_text")]
        public string ButtonText { get; set; }

        [JsonProperty("Themenwelt")]
        public string CategoryId { get; set; }

        public int EndDate { get; set; }

        public string Endtime { get; set; }

        [JsonProperty("Flag_text")]
        public string FlagText { get; set; }

        public string Grundpreis { get; set; }

        public string Id { get; set; }

        public string Logos { get; set; }

        public string Menge { get; set; }

        public string Nan { get; set; }

        public string Preis { get; set; }

        public string Preisalt { get; set; }

        public string Preiszusatz { get; set; }

        public int Productend { get; set; }

        public int Productstart { get; set; }

        public string Regionen { get; set; }

        public int StartDate { get; set; }

        public string Starttime { get; set; }

        public string Titel { get; set; }

        public string Untertitel { get; set; }
    }
}