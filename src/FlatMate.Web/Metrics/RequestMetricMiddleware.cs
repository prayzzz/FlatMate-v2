using App.Metrics;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FlatMate.Web.Metrics
{
    public class RequestMetricMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMetricsRoot _metrics;

        public RequestMetricMiddleware(RequestDelegate next, IMetricsRoot metrics)
        {
            _next = next;
            _metrics = metrics;
        }

        public Task Invoke(HttpContext context)
        {
            var startTime = _metrics.Clock.Nanoseconds;
            var result = _next(context);
            var elapsed = _metrics.Clock.Nanoseconds - startTime;

            _metrics.Measure.Histogram.Update(ModuleMetrics.ResponseTimes, elapsed);
            _metrics.Measure.Meter.Mark(ModuleMetrics.ResponseStatusCodes, context.Response.StatusCode.ToString());

            return result;
        }
    }
}