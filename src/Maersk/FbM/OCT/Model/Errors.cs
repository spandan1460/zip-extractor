using System.Reflection;
using System.Runtime.Serialization;

namespace Maersk.FbM.OCT.Model;
using System.Net;

/// <summary>
/// Defines the errors to be returned via a business logic implementation.
/// </summary>
public class Errors
{
    public ErrorModuleEnum Module { get; set; }
    public HttpStatusCode Code { get; set; }
    public string Message { get; set; }

    public Errors() {
    }

    public Errors(ErrorModuleEnum module, HttpStatusCode code, string message) {
        Module = module;
        Code = code;
        Message = message;
    }

    public string ToStringifiedCode() {
        var module = typeof(ErrorModuleEnum)
            .GetTypeInfo()
            .DeclaredMembers
            .SingleOrDefault(x => x.Name == Module.ToString())
            ?.GetCustomAttribute<EnumMemberAttribute>(false)
            ?.Value;

        return (module + "." + Code).ToUpper();
    }
}
