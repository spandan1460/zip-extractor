namespace Maersk.FbM.OCT.Extensions;

/// <summary>
/// Provides extension methods for string manipulation and sanitization.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Removes newline characters (e.g., line breaks) from the input string and returns the sanitized string.
    /// </summary>
    /// <param name="input">The input string to sanitize.</param>
    /// <returns>A string with newline characters removed.</returns>
    public static string Sanitize(this string input)
        => input.Replace(Environment.NewLine, "");
}