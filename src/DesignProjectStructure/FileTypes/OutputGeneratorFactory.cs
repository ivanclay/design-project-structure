namespace DesignProjectStructure.FileTypes;

/// <summary>
/// Factoryto create output generators based on format
/// </summary>
public static class OutputGeneratorFactory
{
    private static readonly Dictionary<string, Func<IOutputGenerator>> _generators = new()
    {
        { "json", () => new OutputJsonGenerator() },
        { "markdown", () => new OutputMarkdownGenerator() },
        { "md", () => new OutputMarkdownGenerator() },
        { "html", () => new OutputHtmlGenerator() },
        { "htm", () => new OutputHtmlGenerator() }
    };

    /// <summary>
    /// Creates an output generator based on the specified format
    /// </summary>
    /// <param name="format">Desired format (json, markdown, html, etc.)</param>
    /// <returns>Instance of the appropriate generator</returns>
    /// <exception cref="NotSupportedException">When the format is not supported</exception>
    public static IOutputGenerator Create(string format)
    {
        var normalizedFormat = format.ToLowerInvariant().Trim();

        if (_generators.TryGetValue(normalizedFormat, out var generatorFactory))
        {
            return generatorFactory();
        }

        throw new NotSupportedException($"Format '{format}' is not supported. " +
            $"Supported formats: {string.Join(", ", _generators.Keys)}");
    }

    /// <summary>
    /// Checks if a format is supported
    /// </summary>
    /// <param name="format">Format to check</param>
    /// <returns>True if supported, false otherwise</returns>
    public static bool IsSupported(string format)
    {
        var normalizedFormat = format.ToLowerInvariant().Trim();
        return _generators.ContainsKey(normalizedFormat);
    }

    /// <summary>
    /// Returns all supported formats
    /// </summary>
    /// <returns>List of supported formats</returns>
    public static IEnumerable<string> GetSupportedFormats()
    {
        return _generators.Keys.OrderBy(k => k);
    }

    /// <summary>
    /// Register a new generator (for future extensibility)
    /// </summary>
    /// <param name="format">Format name</param>
    /// <param name="generatorFactory">Factory function that creates the generator</param>
    public static void RegisterGenerator(string format, Func<IOutputGenerator> generatorFactory)
    {
        var normalizedFormat = format.ToLowerInvariant().Trim();
        _generators[normalizedFormat] = generatorFactory;
    }

    /// <summary>
    /// Removes support for a format
    /// </summary>
    /// <param name="format">Format to remove</param>
    /// <returns>True if removed, false if not present</returns>
    public static bool UnregisterGenerator(string format)
    {
        var normalizedFormat = format.ToLowerInvariant().Trim();
        return _generators.Remove(normalizedFormat);
    }
}