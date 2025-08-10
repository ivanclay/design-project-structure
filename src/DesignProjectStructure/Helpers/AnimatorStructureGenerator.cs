using DesignProjectStructure.Configuration;
using DesignProjectStructure.Models;

namespace DesignProjectStructure.Helpers;

public class AnimatorStructureGenerator
{
    StructureItens _structureItens;

    public AnimatorStructureGenerator(StructureItens structureItens)
    {
        _structureItens = structureItens;
    }

    public void GenerateStructureAnimated()
    {
        // Call the recursive method with the initial parameters
        GenerateStructureAnimatedRecursive(
            _structureItens.Path,
            _structureItens.Prefix,
            _structureItens.IsLast);
    }

    /// <summary>
    /// Gera a estrutura sem animação - mais rápido para processamento em batch
    /// </summary>
    public void GenerateStructureWithoutAnimation()
    {
        GenerateStructureSilentRecursive(
            _structureItens.Path,
            _structureItens.Prefix,
            _structureItens.IsLast);

        // Atualiza a interface uma única vez no final
        ConsoleRenderer.UpdateProgressBar(100);
        StructureGenerator.UpdateStatus(
            _structureItens.FolderCounter,
            _structureItens.FileCounter,
            _structureItens.ProcessedItems,
            _structureItens.TotalItems);
    }

    private void GenerateStructureSilentRecursive(string path, string prefix, bool isLast)
    {
        try
        {
            var name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(name))
                name = path;

            // Icon for file (Unicode)
            string fileIcon = IconProvider.GetIcon(path);
            string fileLine = $"{prefix}{(isLast ? "└── " : "├── ")}{fileIcon} {name}";

            // Update counters
            if (Directory.Exists(path))
                _structureItens.FolderCounter++;
            else
                _structureItens.FileCounter++;

            _structureItens.ProcessedItems++;

            // Add to the complete structure (for the file) with Unicode icons - SEM linha de cabeçalho
            _structureItens.CompleteStructure.AppendLine(fileLine);

            // Add to visual structure for final display (without console animation)
            string consoleIcon = IconProvider.GetConsoleIcon(path);
            string consoleLinee = $"{prefix}{(isLast ? "└── " : "├── ")}{consoleIcon} {name}";
            _structureItens.VisualStructure.Add(consoleLinee);

            // If it is a directory, process children
            if (Directory.Exists(path))
            {
                var items = Directory.GetFileSystemEntries(path);
                Array.Sort(items, (x, y) =>
                {
                    bool xIsDir = Directory.Exists(x);
                    bool yIsDir = Directory.Exists(y);

                    if (xIsDir && !yIsDir) return -1;
                    if (!xIsDir && yIsDir) return 1;

                    return Path.GetFileName(x).CompareTo(Path.GetFileName(y));
                });

                var validItems = new List<string>();
                foreach (var item in items)
                {
                    if (!IgnoreFilter.MustIgnore(Path.GetFileName(item)))
                        validItems.Add(item);
                }

                for (int i = 0; i < validItems.Count; i++)
                {
                    string newPrefix = prefix + (isLast ? "    " : "│   ");
                    bool isLastItem = (i == validItems.Count - 1);

                    // Recursive call with local parameters
                    GenerateStructureSilentRecursive(validItems[i], newPrefix, isLastItem);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Para exceções, usar ícones Unicode no arquivo
            string fileLine = $"{prefix}└── 🔒 [Access denied]";
            _structureItens.CompleteStructure.AppendLine(fileLine);

            string consoleLine = $"{prefix}└── [LOCK] [Access denied]";
            _structureItens.VisualStructure.Add(consoleLine);

            _structureItens.ProcessedItems++;
        }
        catch (Exception ex)
        {
            // Para exceções, usar ícones Unicode no arquivo
            string fileLine = $"{prefix}└── ❌ [Erro: {ex.Message}]";
            _structureItens.CompleteStructure.AppendLine(fileLine);

            string consoleLine = $"{prefix}└── [ERR] [Erro: {ex.Message}]";
            _structureItens.VisualStructure.Add(consoleLine);

            _structureItens.ProcessedItems++;
        }
    }

    private void GenerateStructureAnimatedRecursive(string path, string prefix, bool isLast)
    {
        try
        {
            var config = ConfigurationManager.Instance.Config;
            var name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(name))
                name = path;

            // Icon for console (plain text)
            string consoleIcon = IconProvider.GetConsoleIcon(path);
            string linhaConsole = $"{prefix}{(isLast ? "└── " : "├── ")}{consoleIcon} {name}";

            // Icon for file (Unicode)
            string fileIcon = IconProvider.GetIcon(path);
            string fileLine = $"{prefix}{(isLast ? "└── " : "├── ")}{fileIcon} {name}";

            // Update counters
            if (Directory.Exists(path))
                _structureItens.FolderCounter++;
            else
                _structureItens.FileCounter++;

            _structureItens.ProcessedItems++;

            // Add to the complete structure (for the file) with Unicode icons
            _structureItens.CompleteStructure.AppendLine(fileLine);

            // Updates interface (in console) with text icons
            StructureGenerator.UpdateStructure(linhaConsole, _structureItens.VisualStructure);
            StructureGenerator.UpdateStatus(
                _structureItens.FolderCounter,
                _structureItens.FileCounter,
                _structureItens.ProcessedItems,
                _structureItens.TotalItems);

            // If it is a directory, process children
            if (Directory.Exists(path))
            {
                var items = Directory.GetFileSystemEntries(path);
                Array.Sort(items, (x, y) =>
                {
                    bool xIsDir = Directory.Exists(x);
                    bool yIsDir = Directory.Exists(y);

                    if (xIsDir && !yIsDir) return -1;
                    if (!xIsDir && yIsDir) return 1;

                    return Path.GetFileName(x).CompareTo(Path.GetFileName(y));
                });

                var validItems = new List<string>();
                foreach (var item in items)
                {
                    if (!IgnoreFilter.MustIgnore(Path.GetFileName(item)))
                        validItems.Add(item);
                }

                for (int i = 0; i < validItems.Count; i++)
                {
                    string newPrefix = prefix + (isLast ? "    " : "│   ");
                    bool isLastItem = (i == validItems.Count - 1);

                    // Recursive call with local parameters
                    GenerateStructureAnimatedRecursive(validItems[i], newPrefix, isLastItem);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // For exceptions, use simple icons in the console and Unicode in the file
            string consoleLine = $"{prefix}└── [LOCK] [Access denied]";
            string fileLine = $"{prefix}└── 🔒 [Access denied]";

            _structureItens.CompleteStructure.AppendLine(fileLine);
            StructureGenerator.UpdateStructure(consoleLine, _structureItens.VisualStructure);
            _structureItens.ProcessedItems++;
            StructureGenerator.UpdateStatus(
                _structureItens.FolderCounter,
                _structureItens.FileCounter,
                _structureItens.ProcessedItems,
                _structureItens.TotalItems);
        }
        catch (Exception ex)
        {
            // For exceptions, use simple icons in the console and Unicode in the file
            string consoleLine = $"{prefix}└── [ERR] [Erro: {ex.Message}]";
            string fileLine = $"{prefix}└── ❌ [Erro: {ex.Message}]";

            _structureItens.CompleteStructure.AppendLine(fileLine);
            StructureGenerator.UpdateStructure(consoleLine, _structureItens.VisualStructure);
            _structureItens.ProcessedItems++;
            StructureGenerator.UpdateStatus(
                _structureItens.FolderCounter,
                _structureItens.FileCounter,
                _structureItens.ProcessedItems,
                _structureItens.TotalItems);
        }
    }
}