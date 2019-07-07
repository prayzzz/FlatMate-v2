using System;
using FlatMate.Module.Common.Extensions;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Offers;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Offers.Domain.Import.Aldi
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

            return new OfferDuration(from, to);
        }
    }
}