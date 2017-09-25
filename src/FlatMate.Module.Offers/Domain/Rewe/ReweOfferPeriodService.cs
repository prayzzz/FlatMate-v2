using prayzzz.Common.Attributes;
using FlatMate.Module.Common.Extensions;
using System;

namespace FlatMate.Module.Offers.Domain
{
    [Inject]
    public class ReweOfferPeriodService : IOfferPeriodService
    {
        private readonly DayOfWeek _startDay = DayOfWeek.Sunday;
        private readonly DayOfWeek _endDay = DayOfWeek.Saturday;

        public Company Company => Company.Rewe;

        public OfferPeriod ComputeOfferPeriod(DateTime date)
        {
            var from = date.GetPreviousWeekday(_startDay);
            var to = date.GetNextWeekday(_endDay);

            return new OfferPeriod { From = from, To = to };
        }
    }
}