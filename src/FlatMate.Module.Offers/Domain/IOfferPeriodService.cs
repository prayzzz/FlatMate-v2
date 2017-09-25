using System;

namespace FlatMate.Module.Offers.Domain
{
    public interface IOfferPeriodService
    {
        Company Company { get; }

        OfferPeriod ComputeOfferPeriod(DateTime date);
    }
}