using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Maersk.FbM.OCT.Extensions;

/// <summary>
/// Provides extension methods for mapping and retrieving the informational version of an assembly.
/// </summary>
[ExcludeFromCodeCoverage]
public static class VersionExtensions
{
    /// <summary>
    /// Maps a route for retrieving the informational version of the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <param name="pattern">The URL pattern for accessing the version information (default is "/version").</param>
    public static void MapVersion(this WebApplication app, string pattern = "/version") 
        => app.MapGet(pattern, () => GetInformationalVersion());
    
    /// <summary>
    /// Retrieves the informational version of the specified assembly or the entry assembly.
    /// </summary>
    /// <param name="assembly">The assembly for which to retrieve the informational version (default is the entry assembly).</param>
    /// <returns>The informational version of the assembly as a string, or an empty string if not found.</returns>
    public static string GetInformationalVersion(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly();
        if (assembly is null)
        {
            return string.Empty;
        }

        return FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion ?? string.Empty;
    }
}