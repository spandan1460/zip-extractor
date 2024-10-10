using System.Diagnostics.CodeAnalysis;
using Maersk.FbM.OCT.Extensions;

namespace Maersk.FbM.OCT.Middlewares;

/// <summary>
/// Middleware for logging requests and responses in an ASP.NET Core application.
/// </summary>
[ExcludeFromCodeCoverage]
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the LoggingMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="logger">The logger for recording log information.</param>
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Middleware method responsible for processing incoming HTTP requests. 
    /// It logs relevant information, including client name and request ID (X-MAERSK-RID),
    /// after sanitizing them to prevent security vulnerabilities,
    /// and then passes the request to the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The HttpContext representing the current HTTP request.</param>
    /// <returns>A Task representing the asynchronous execution of the middleware.</returns>
    public async Task Invoke(HttpContext context)
    {
        var clientName = context.GetClientName();
        var maerskRid = context.GetOrAddRid();
        
        using (_logger.BeginScope("{ClientName} {X-MAERSK-RID}", clientName.Sanitize(), maerskRid.Sanitize()))
        {
            await _next(context);
        }
    }
}