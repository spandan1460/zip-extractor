using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Maersk.FbM.OCT.Swagger;

/// <summary>
/// </summary>
public class RequestIDHeaderParameter : IOperationFilter
{
    

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameter = new OpenApiParameter();
        parameter.Name = RequestIdMgr.HEADER_NAME;
        parameter.In = ParameterLocation.Header;
        parameter.Description = "CorrelationID to trace requests thru logging within service calls";
        parameter.Required = false;
        operation.Parameters.Add(parameter);
    }
    
}