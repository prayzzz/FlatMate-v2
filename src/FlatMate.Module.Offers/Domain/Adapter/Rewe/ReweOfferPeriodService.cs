﻿using FlatMate.Module.Common.Extensions;
using prayzzz.Common.Attributes;
using System;

namespace FlatMate.Module.Offers.Domain.Adapter.Rewe
{
    [Inject]
    public class ReweOfferPeriodService : IOfferPeriodService
    {
        private readonly DayOfWeek _endDay = DayOfWeek.Sunday;
        private readonly DayOfWeek _startDay = DayOfWeek.Monday;

        public Company Company => Company.Rewe;

        public OfferDuration ComputeOfferPeriod(DateTime date)
        {
            var from = date.GetPreviousWeekday(_startDay);
            var to = date.GetNextWeekday(_endDay);

            return new OfferDuration { From = from, To = to };
        }
    }
}