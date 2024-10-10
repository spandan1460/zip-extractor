using System.Diagnostics.CodeAnalysis;
using Maersk.FbM.OCT.HealthCheck;

namespace Maersk.FbM.OCT.Extensions;

/// <summary>
/// Provides extension methods for configuring health checks in the IServiceCollection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HealthChecksExtensions
{
    /// <summary>
    /// Adds API-specific health checks to the IServiceCollection. These health checks are used to verify the operational status of the service.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    public static void AddApiHealthChecks(this IServiceCollection services)
    {
        // Add Health checks to verify the service is operational
        // Health checks should check the integrity of the micro-service, NOT the dependencies of the micro-service.
        // If a health-check verifies its dependencies, then when those dependencies fail, all of the nodes will go down -
        // instead a health check should verify that the health of the existing service (memory, disk, I/O is of sufficient
        // health) in order to serve traffic.
        services
            .AddHealthChecks()
            .AddCheck<SystemHealthCheck>("System Health Check");
    }
}