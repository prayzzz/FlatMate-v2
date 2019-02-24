using System;

namespace FlatMate.Module.Offers.Domain.Offers
{
    public class OfferDuration
    {
        public OfferDuration(DateTime from, DateTime to)
        {
            From = from.Date;
            To = to.Date;
        }

        public DateTime From { get; }

        public DateTime To { get; }
    }
}