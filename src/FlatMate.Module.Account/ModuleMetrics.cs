using App.Metrics;
using App.Metrics.Meter;

namespace FlatMate.Module.Account
{
    public static class ModuleMetrics
    {
        private const string ModulePrefix = "FlatMate.Module.Account.";

        public static MeterOptions LoginAttempts => new MeterOptions
        {
            Name = ModulePrefix + nameof(LoginAttempts),
            MeasurementUnit = Unit.Calls
        };
    }
}
