using System.Diagnostics.CodeAnalysis;
using Maersk.FbM.OCT.Middlewares;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Maersk.FbM.OCT.Extensions;

/// <summary>
/// Provides extension methods for configuring and using logging middleware and services.
/// </summary>
[ExcludeFromCodeCoverage]
public static class LoggingExtensions
{
    /// <summary>
    /// Adds logging middleware to the application builder when specified conditions are met.
    /// </summary>
    /// <param name="builder">The IApplicationBuilder instance.</param>
    public static void UseLoggingMiddleware(this IApplicationBuilder builder)
        => builder.UseWhen(
            ShouldUseLoggingMiddleware,
            applicationBuilder => applicationBuilder.UseMiddleware<LoggingMiddleware>());

    /// <summary>
    /// Adds logging services to the IServiceCollection with OpenTelemetry support.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="configureResource">Action to configure the resource for OpenTelemetry.</param>
    /// <param name="otlpEndpoint">The OpenTelemetry Protocol (OTLP) endpoint.</param>
    public static void AddLogging(this IServiceCollection services, Action<ResourceBuilder> configureResource,
        string otlpEndpoint)
    {
        services.AddLogging(builder =>
        {
            builder
                .ClearProviders()
                .AddOpenTelemetry(options =>
                {
                    var resourceBuilder = ResourceBuilder.CreateDefault();
                    configureResource(resourceBuilder);
                    options.IncludeScopes = true;
                    options.SetResourceBuilder(resourceBuilder);
                    options.AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(otlpEndpoint));
                });
#if DEBUG
            builder
                .AddDebug()
                .AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.ffff] ";
                    options.UseUtcTimestamp = true;
                    options.SingleLine = true;
                    options.IncludeScopes = true;
                });
#endif
        });
    }
    
    private static bool ShouldUseLoggingMiddleware(HttpContext context)
    {
        var path = context.Request.Path.Value;
        return !string.IsNullOrEmpty(path) &&
               !path.Contains("health", StringComparison.InvariantCultureIgnoreCase) &&
               !path.Contains("version", StringComparison.InvariantCultureIgnoreCase);
    }
}