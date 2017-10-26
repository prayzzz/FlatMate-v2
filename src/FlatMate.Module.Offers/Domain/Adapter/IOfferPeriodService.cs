using System;

namespace FlatMate.Module.Offers.Domain.Adapter
{
    public interface IOfferPeriodService
    {
        Company Company { get; }

        OfferDuration ComputeOfferPeriod(DateTime date);
    }
}
