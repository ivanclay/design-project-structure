using DesignProjectStructure.Helpers;
using DesignProjectStructure.Models;

namespace DesignProjectStructure.FileTypes;

/// <summary>
/// Markdown output generator
/// </summary>
public class OutputMarkdownGenerator : IOutputGenerator
{
    public string Generate(StructureItens structureItens, string rootPath)
    {
        // Markdown uses the complete framework that has already been built
        // with Unicode icons during processing
        return structureItens.CompleteStructure.ToString();
    }

    public string GetFileExtension() => "md";

    public string GetFormatName() => "Markdown";

    public bool SupportsFormat(string format) =>
        format.Equals("markdown", StringComparison.OrdinalIgnoreCase) ||
        format.Equals("md", StringComparison.OrdinalIgnoreCase);
}