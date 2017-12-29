using System;
using FlatMate.Module.Common.Extensions;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Offers.Domain.Adapter.Aldi
{
    [Inject]
    public class AldiOfferPeriodService : IOfferPeriodService
    {
        private const DayOfWeek EndDay = DayOfWeek.Sunday;
        private const DayOfWeek StartDay = DayOfWeek.Monday;

        public Company Company => Company.AldiNord;

        public OfferDuration ComputeOfferPeriod(DateTime date)
        {
            var from = date.GetPreviousWeekday(StartDay);
            var to = date.GetNextWeekday(EndDay);

            return new OfferDuration { From = from, To = to };
        }
    }
}