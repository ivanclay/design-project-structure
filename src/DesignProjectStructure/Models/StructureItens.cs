using System.Text;

namespace DesignProjectStructure.Models;

public class StructureItens
{
    public string Path { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public bool IsLast { get; set; }
    public int FolderCounter { get; set; }
    public int FileCounter { get; set; }
    public int ProcessedItems { get; set; }
    public StringBuilder CompleteStructure { get; set; } = new StringBuilder();
    public List<string> VisualStructure { get; set; } = new List<string>();
    public int TotalItems { get; set; } = 0;
}
