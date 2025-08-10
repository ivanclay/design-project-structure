using DesignProjectStructure.Models;

namespace DesignProjectStructure.FileTypes;

/// <summary>
/// Common interface for all output generators
/// </summary>
public interface IOutputGenerator
{
    /// <summary>
    /// Generates content in a specific format
    /// </summary>
    /// <param name="structureItens">Processed structure data</param>
    /// <param name="rootPath">Project root path</param>
    /// <returns>Content formatted as a string</returns>
    string Generate(StructureItens structureItens, string rootPath);

    /// <summary>
    /// File extension for this format
    /// </summary>
    string GetFileExtension();

    /// <summary>
    /// Friendly format name
    /// </summary>
    string GetFormatName();

    /// <summary>
    /// Indicates whether this generator supports the specified format
    /// </summary>
    /// <param name="format">Format name</param>
    bool SupportsFormat(string format);
}