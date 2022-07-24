using Prometheus;

namespace AssignmentService.Server;

public static class PrometheusMetrics
{
    internal static readonly Counter FailedDownstreamRequests = Metrics.CreateCounter(
        $"{ServiceConfigOptions.AppPrefix}_failed_downstream_requests",
        "Total number of failed requests to downstream services",
        new CounterConfiguration
        {
            LabelNames = new[] { "service" }
        });

    internal static readonly Counter ServedContentTypes = Metrics.CreateCounter(
        $"{ServiceConfigOptions.AppPrefix}_served_content_types",
        "Total number of served requests by content type",
        new CounterConfiguration
        {
            LabelNames = new[] { "content_type" }
        });

    internal static readonly Counter CorruptedPdfs =
        Metrics.CreateCounter($"{ServiceConfigOptions.AppPrefix}_corrupted_pdfs",
            "Total number of corrupted PDF files served");

    internal static readonly Gauge AppInfo = Metrics.CreateGauge($"{ServiceConfigOptions.AppPrefix}_app_info",
        "Application info including version",
        new GaugeConfiguration
        {
            LabelNames = new[] { "version" }
        });
}
