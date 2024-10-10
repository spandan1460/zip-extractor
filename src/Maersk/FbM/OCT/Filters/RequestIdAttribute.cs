using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace Maersk.FbM.OCT.Filters;

/// <summary>
/// Defines a filter for intercepting HTTP requests, pulling out the X-MAERSK-RID header from the request and setting
/// the NLog 'rid' field so that it can be logged by any configured appender.
/// </summary>
public class RequestIdAttribute : ActionFilterAttribute
{
    private readonly Logger _logger = NLog.LogManager.Setup().GetCurrentClassLogger();
 
    /// <summary>
    /// Pulls out or generates the RID field out of the HTTP context (request header) and places it into the NLog rid
    /// dynamic value.
    /// </summary>
    /// <param name="filterContext">The filter context chain used to invoke the next element in the sequence</param>
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var context = filterContext.HttpContext;
        string requestId = RequestIdMgr.RetrieveOrGenerate(context);
        _logger.Info($"OnActionExecuted: Start of request requestId={requestId}");
        base.OnActionExecuting(filterContext);
    }

    /// <summary>
    /// Resets the NLog filter value and calls the next executed filter in the chain.
    /// </summary>
    /// <param name="filterContext">The filter context chain used to invoke the next element in the sequence</param>
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        // Clear the RID
        RequestIdMgr.Clear();
        base.OnResultExecuted(filterContext);
    }
    
}