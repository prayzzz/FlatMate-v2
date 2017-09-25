using System;

namespace FlatMate.Module.Offers.Domain.Rewe
{
    public class OfferDurationJso
    {
        public DateTime From { get; set; }

        public int RemainingDays { get; set; }

        public DateTime Until { get; set; }
    }
}