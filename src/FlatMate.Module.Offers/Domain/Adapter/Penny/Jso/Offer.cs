using Newtonsoft.Json;

namespace FlatMate.Module.Offers.Domain.Adapter.Penny.Jso
{
    public class OfferJso
    {
        public string Beschreibung { get; set; }

        public string Bild { get; set; }

        public string Bild_full { get; set; }

        public string Bild_high { get; set; }

        public string Bild_normal { get; set; }

        public string Bild_original { get; set; }

        public string Bild_preview { get; set; }

        public string Bilder { get; set; }

        public string Button_link { get; set; }

        public string Button_text { get; set; }

        [JsonProperty("Themenwelt")]
        public string CategoryId { get; set; }

        public int EndDate { get; set; }

        public string Endtime { get; set; }

        public string Flag_text { get; set; }

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