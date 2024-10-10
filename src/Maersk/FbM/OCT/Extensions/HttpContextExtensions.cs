using System.Diagnostics;
using System.Text.Json;
using Maersk.FbM.OCT.Contracts;

namespace Maersk.FbM.OCT.Extensions;

/// <summary>
/// Provides extension methods for working with HttpContext objects.
/// </summary>
public static class HttpContextExtensions
{
    private const string HeaderName = "X-MAERSK-RID";

    /// <summary>
    /// Retrieves the client name from the HttpContext's request query.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The client name as a string, or an empty string if not found.</returns>
    public static string GetClientName(this HttpContext context)
    {
        var clientInfoAsString = context.Request.Query["Client"]
            .ToString();

        if (string.IsNullOrEmpty(clientInfoAsString))
        {
            return string.Empty;
        }

        var clientInfo = JsonSerializer.Deserialize<ClientInfo>(clientInfoAsString);

        return clientInfo is null ? string.Empty : clientInfo.Name;
    }
    
    /// <summary>
    /// Retrieves or generates a request ID (RID) from the HttpContext's request and response headers.
    /// </summary>
    /// <param name="context">The HttpContext instance.</param>
    /// <returns>The request ID as a string.</returns>
    public static string GetOrAddRid(this HttpContext context)
    {
        var requestId = context.Request.Headers
            .FirstOrDefault(x => string.Equals(x.Key, HeaderName, StringComparison.InvariantCultureIgnoreCase)).Value
            .ToString();

        if (string.IsNullOrEmpty(requestId))
        {
            requestId = GenerateRid();
        }

        AddRequestIdToResponseHeaders(context, requestId);

        return requestId;
    }

    private static void AddRequestIdToResponseHeaders(HttpContext context, string requestId)
    {
        if (!context.Response.Headers.ContainsKey(HeaderName))
        {
            context.Response.Headers.Add(HeaderName, requestId);
        }
    }

    private static string GenerateRid()
        => Activity.Current is not null ? Activity.Current.TraceId.ToString() : Guid.NewGuid().ToString();
}