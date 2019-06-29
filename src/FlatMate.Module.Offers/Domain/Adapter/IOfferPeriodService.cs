﻿using System;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Offers;

namespace FlatMate.Module.Offers.Domain.Adapter
{
    public interface IOfferPeriodService
    {
        Company Company { get; }

        OfferDuration ComputeOfferPeriod(DateTime date);
    }
}