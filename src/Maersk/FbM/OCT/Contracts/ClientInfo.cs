namespace Maersk.FbM.OCT.Contracts;

/// <summary>
/// Represents client information with various properties.
/// </summary>
public class ClientInfo
{
    public string Name { get; set; } = string.Empty;
    public string Guid { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}