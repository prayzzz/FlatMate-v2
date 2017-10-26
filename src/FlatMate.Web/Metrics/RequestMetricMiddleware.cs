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
            var result = _next(context);

            _metrics.Measure.Meter.Mark(ModuleMetrics.ResponseStatusCodes, context.Response.StatusCode.ToString());

            return result;
        }
    }
}
