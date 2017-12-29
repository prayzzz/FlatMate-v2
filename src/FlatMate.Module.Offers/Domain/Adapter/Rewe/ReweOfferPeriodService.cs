using FlatMate.Module.Common.Extensions;
using prayzzz.Common.Attributes;
using System;

namespace FlatMate.Module.Offers.Domain.Adapter.Rewe
{
    [Inject]
    public class ReweOfferPeriodService : IOfferPeriodService
    {
        private const DayOfWeek EndDay = DayOfWeek.Sunday;
        private const DayOfWeek StartDay = DayOfWeek.Monday;

        public Company Company => Company.Rewe;

        public OfferDuration ComputeOfferPeriod(DateTime date)
        {
            var from = date.GetPreviousWeekday(StartDay);
            var to = date.GetNextWeekday(EndDay);

            return new OfferDuration { From = from, To = to };
        }
    }
}
