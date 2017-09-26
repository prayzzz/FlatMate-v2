using System;
namespace FlatMate.Module.Offers.Domain
{
    public class OfferDuration
    {
        private DateTime _from;
        private DateTime _to;

        public DateTime From
        {
            get { return _from; }
            set { _from = value.Date; }
        }

        public DateTime To
        {
            get { return _to; }
            set { _to = value.Date; }
        }
    }
}