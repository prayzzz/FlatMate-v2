using System;
using App.Metrics;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.Formatters.InfluxDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlatMate.Web.Common
{
    public static class MetricsSetup
    {
        private const string MetricsEndpointKey = "Kestrel:Endpoints:MetricHttp:Url";

        private static readonly MetricsOptions MetricsOptions = new MetricsOptions
        {
            Enabled = true,
            GlobalTags = new GlobalMetricTags { { "service", "FlatMate" } }
        };

        public static IServiceCollection AddFlatMateMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            var metricsEndpoint = configuration.GetValue<string>(MetricsEndpointKey);
            if (!string.IsNullOrEmpty(metricsEndpoint))
            {
                services.Configure<MetricsEndpointsHostingOptions>(o => o.MetricsEndpointPort = new Uri(metricsEndpoint).Port);
            }

            var metricsRoot = new MetricsBuilder().SampleWith.ForwardDecaying()
                                                  .TimeWith.StopwatchClock()
                                                  .Configuration.Configure(MetricsOptions)
                                                  .Build();

            services.AddMetrics(metricsRoot);
            services.AddMetricsEndpoints(o => o.MetricsEndpointOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter(new MetricFields()));
            services.AddMetricsTrackingMiddleware(o => o.ApdexTrackingEnabled = false);

            return services;
        }

        public static IApplicationBuilder UseFlatMateMetrics(this IApplicationBuilder app)
        {
            app.UseMetricsRequestTrackingMiddleware();
            app.UseMetricsErrorTrackingMiddleware();
            app.UseMetricsEndpoint();

            return app;
        }
    }
}