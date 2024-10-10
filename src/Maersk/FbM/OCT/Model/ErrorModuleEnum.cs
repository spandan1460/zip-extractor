using System.Runtime.Serialization;

namespace Maersk.FbM.OCT.Model;

/// <summary>
/// List of error types.
/// </summary>
public enum ErrorModuleEnum
{
    [EnumMember(Value = "Alert Proxy Failure")]
    ALERT_PROXY_FALURE = 0,
}