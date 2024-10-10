using Microsoft.Extensions.Primitives;

namespace Maersk.FbM.OCT;

/// <summary>
/// Provides a correlationId (requestId) for client consumption by first consuming the value observed in X-MAERSK-RID
/// creating a
///     <b>serviceName</b> + "Client=<<X-MAERSK-RID value from client>>;" + <b>serviceName</b> + "=<i>GUID</i>"
/// </summary>
public class RequestIdMgr
{
    public const string HEADER_NAME = "X-MAERSK-RID";
    public const string LOG_RID = "rid";
    
    // TODO: Replace with the name of this service
    public const string SERVICE_NAME = "weather";

    /// <summary>
    /// Clear the rid from the Global diagnostics context in NLog so that it does not leak into other logging beyond this
    /// request execution.
    /// </summary>
    public static void Clear()
    {
        NLog.GlobalDiagnosticsContext.Set(LOG_RID, "");   
    }
    
    
    /// <summary>
    /// Retrieves the X-MAERSK-RID from the HTTP headers, or alternatively generates a guid to inject in the reply.
    /// 
    /// This creates a chained requestId format, where-in the originating client rid is supplied first (if available)
    /// then the weather service (this service's).
    /// </summary>
    public static string RetrieveOrGenerate(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HEADER_NAME, out StringValues requestId))
        {
            requestId = GenerateRID();
        }
        else
        {
            requestId = SERVICE_NAME + "Client=" + requestId + ";" + SERVICE_NAME + "=" + Guid.NewGuid().ToString();
        }
        context.Response.Headers.Add(HEADER_NAME, requestId); 
        NLog.GlobalDiagnosticsContext.Set(LOG_RID, requestId);
        context.Items[LOG_RID] = requestId;
        return requestId;
    }

    /// <summary>
    /// Obtain the current RID for the purpose of passing it along to another called service if this service is a proxy
    /// around another micro-service.
    /// </summary>
    public static string GetCurrent()
    {
        return NLog.GlobalDiagnosticsContext.Get(LOG_RID);
    }

    /// <summary>
    /// Generates a requestID 
    /// </summary>
    private static string GenerateRID()
    {
        return Guid.NewGuid().ToString();
    }
}