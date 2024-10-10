using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Maersk.FbM.OCT.Extensions;

/// <summary>
/// Provides extension methods for configuring and adding OpenTelemetry instrumentation and exporters.
/// </summary>
[ExcludeFromCodeCoverage]
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry instrumentation and exporters to the IServiceCollection for tracing and metrics.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="configureResource">Action to configure the resource for OpenTelemetry.</param>
    /// <param name="otlpEndpoint">The OpenTelemetry Protocol (OTLP) endpoint.</param>
    public static void AddOpenTelemetry(this IServiceCollection services,
        Action<ResourceBuilder> configureResource, string otlpEndpoint)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(configureResource)
            .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = HttpContextFilter;
                })
                .AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                })
                .AddEntityFrameworkCoreInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                })
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otlpEndpoint);
                }))
            .WithMetrics(metricsProviderBuilder => metricsProviderBuilder
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otlpEndpoint);
                }));
    }

    private static bool HttpContextFilter(HttpContext httpContext)
    {
        var requestPath = httpContext.Request.Path.ToUriComponent();
        return !requestPath.Contains("index.html", StringComparison.OrdinalIgnoreCase)
               && !requestPath.Contains("swagger", StringComparison.OrdinalIgnoreCase);
    }
}