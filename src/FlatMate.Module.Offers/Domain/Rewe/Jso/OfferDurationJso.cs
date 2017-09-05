using System;

namespace FlatMate.Module.Offers.Domain.Rewe.Jso
{
    public class OfferDurationJso
    {
        public DateTime From { get; set; }

        public int RemainingDays { get; set; }

        public DateTime Until { get; set; }
    }
}