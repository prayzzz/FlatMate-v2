﻿using App.Metrics;
using App.Metrics.Histogram;
using App.Metrics.Meter;

namespace FlatMate.Web
{
    public static class ModuleMetrics
    {
        public static MeterOptions ResponseStatusCodes => new MeterOptions
        {
            Name = "FlatMate.Web." + nameof(ResponseStatusCodes),
            MeasurementUnit = Unit.Calls
        };

        public static HistogramOptions ResponseTimes => new HistogramOptions
        {
            Name = "FlatMate.Web." + nameof(ResponseTimes),
            MeasurementUnit = Unit.Calls
        };
    }
}