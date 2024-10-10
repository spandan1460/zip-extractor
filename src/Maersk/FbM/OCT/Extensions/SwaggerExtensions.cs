using System.Diagnostics.CodeAnalysis;
using Maersk.FbM.OCT.Swagger;
using Microsoft.OpenApi.Models;

namespace Maersk.FbM.OCT.Extensions;

/// <summary>
/// Provides extension methods for configuring Swagger documentation.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger documentation generation to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Weather API",
                Description = "A FbM Example .NET7 Core Web API for interacting with a weather service",
            });
            options.OperationFilter<RequestIDHeaderParameter>();
        });
    }
}