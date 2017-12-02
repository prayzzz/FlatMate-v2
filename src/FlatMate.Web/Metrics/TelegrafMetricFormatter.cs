using System;
using App.Metrics;
using App.Metrics.Formatters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FlatMate.Web.Metrics
{
    public class MetricsEnvelop
    {
        public MetricsEnvelop(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }

        public DateTime TimeStamp { get; }

        public Dictionary<string, object> Counters { get; } = new Dictionary<string, object>();

        public Dictionary<string, object> Gauges { get; } = new Dictionary<string, object>();

        public Dictionary<string, object> Meters { get; } = new Dictionary<string, object>();

        public Dictionary<string, object> Histogram { get; } = new Dictionary<string, object>();
    }

    public class TelegrafMetricFormatter : IMetricsOutputFormatter
    {
        public MetricsMediaTypeValue MediaType => new MetricsMediaTypeValue("application", "vnd.custom.metrics", "v1", "json");

        public Task WriteAsync(Stream output, MetricsDataValueSource metricsData, CancellationToken cancellationToken = default(CancellationToken))
        {
            var envelop = new MetricsEnvelop(metricsData.Timestamp);

            foreach (var context in metricsData.Contexts)
            {
                foreach (var counters in context.Counters)
                {
                    envelop.Counters.Add(counters.Name, counters.Value.Count);
                }

                foreach (var gauges in context.Gauges)
                {
                    envelop.Gauges.Add(gauges.Name, gauges.Value);
                }

                foreach (var histogram in context.Histograms)
                {
                    envelop.Histogram.Add(histogram.Name + "." + "mean", histogram.Value.Mean);
                    envelop.Histogram.Add(histogram.Name + "." + "median", histogram.Value.Median);
                    envelop.Histogram.Add(histogram.Name + "." + "p95", histogram.Value.Percentile95);
                    envelop.Histogram.Add(histogram.Name + "." + "p98", histogram.Value.Percentile98);
                    envelop.Histogram.Add(histogram.Name + "." + "p99", histogram.Value.Percentile99);
                }

                foreach (var meter in context.Meters)
                {
                    foreach (var item in meter.Value.Items)
                    {
                        envelop.Meters.Add(meter.Name + "." + item.Item, item.Value);
                    }
                }
            }

            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(output))
            {
                serializer.Serialize(writer, envelop);
            }

            return Task.CompletedTask;
        }
    }
}
