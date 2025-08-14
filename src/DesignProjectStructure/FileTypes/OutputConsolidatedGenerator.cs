using DesignProjectStructure.Configuration;
using DesignProjectStructure.Helpers;
using DesignProjectStructure.Models;
using System.Text;

namespace DesignProjectStructure.FileTypes;

/// <summary>
/// Gerador que consolida todos os arquivos de código em um único documento
/// Ideal para enviar contexto completo para assistentes de IA
/// </summary>
public class OutputConsolidatedGenerator : IOutputGenerator
{
    private readonly HashSet<string> _supportedExtensions = new()
    {
        ".cs", ".js", ".ts", ".py", ".java", ".cpp", ".c", ".h", ".hpp",
        ".php", ".rb", ".go", ".rs", ".kt", ".swift", ".dart", ".scala",
        ".html", ".css", ".scss", ".less", ".sql", ".xml", ".json", ".yaml", ".yml",
        ".md", ".txt", ".config", ".ini", ".toml", ".sh", ".bat", ".ps1",
        ".vue", ".jsx", ".tsx", ".svelte", ".razor", ".cshtml", ".vbhtml"
    };

    public string Generate(StructureItens structureItens, string rootPath)
    {
        var config = ConfigurationManager.Instance.Config;
        var consolidatedContent = new StringBuilder();

        // Cabeçalho do documento
        GenerateHeader(consolidatedContent, structureItens, rootPath);

        // Índice de arquivos
        var codeFiles = GetCodeFiles(rootPath);
        GenerateIndex(consolidatedContent, codeFiles, rootPath);

        // Separador
        consolidatedContent.AppendLine();
        consolidatedContent.AppendLine(new string('=', 80));
        consolidatedContent.AppendLine("# ARQUIVOS DO PROJETO");
        consolidatedContent.AppendLine(new string('=', 80));
        consolidatedContent.AppendLine();

        // Conteúdo dos arquivos
        GenerateFileContents(consolidatedContent, codeFiles, rootPath);

        return consolidatedContent.ToString();
    }

    public string GetFileExtension() => "md";

    public string GetFormatName() => "Consolidated Code";

    public bool SupportsFormat(string format) =>
        format.Equals("consolidated", StringComparison.OrdinalIgnoreCase) ||
        format.Equals("single", StringComparison.OrdinalIgnoreCase) ||
        format.Equals("all-in-one", StringComparison.OrdinalIgnoreCase);

    #region Métodos privados

    private void GenerateHeader(StringBuilder content, StructureItens structureItens, string rootPath)
    {
        var projectInfo = ProjectTypeDetector.DetectProjectType(rootPath);

        content.AppendLine("# PROJETO CONSOLIDADO PARA ANÁLISE DE IA");
        content.AppendLine();
        content.AppendLine("Este documento contém todos os arquivos de código do projeto em um único arquivo,");
        content.AppendLine("facilitando o upload e análise por assistentes de IA.");
        content.AppendLine();
        content.AppendLine("## INFORMAÇÕES DO PROJETO");
        content.AppendLine();
        content.AppendLine($"- **Nome:** {Path.GetFileName(rootPath)}");
        content.AppendLine($"- **Caminho:** {rootPath}");
        content.AppendLine($"- **Tipo:** {projectInfo.Type}");
        content.AppendLine($"- **Linguagem:** {projectInfo.Language}");

        if (!string.IsNullOrEmpty(projectInfo.Framework) && projectInfo.Framework != projectInfo.Language)
        {
            content.AppendLine($"- **Framework:** {projectInfo.Framework}");
        }

        content.AppendLine($"- **Descrição:** {projectInfo.Description}");
        content.AppendLine($"- **Data de Geração:** {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        content.AppendLine();

        content.AppendLine("## ESTATÍSTICAS");
        content.AppendLine();
        content.AppendLine($"- **Total de Pastas:** {structureItens.FolderCounter}");
        content.AppendLine($"- **Total de Arquivos:** {structureItens.FileCounter}");
        content.AppendLine($"- **Itens Processados:** {structureItens.ProcessedItems}");
        content.AppendLine();
    }

    private List<FileInfo> GetCodeFiles(string rootPath)
    {
        var codeFiles = new List<FileInfo>();

        try
        {
            var allFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

            foreach (var filePath in allFiles)
            {
                var fileName = Path.GetFileName(filePath);
                var extension = Path.GetExtension(filePath).ToLower();

                // Ignora arquivos conforme configuração
                if (IgnoreFilter.MustIgnore(fileName))
                    continue;

                // Apenas arquivos de código suportados
                if (!_supportedExtensions.Contains(extension))
                    continue;

                // Ignora arquivos muito grandes (> 1MB)
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 1024 * 1024)
                    continue;

                codeFiles.Add(fileInfo);
            }
        }
        catch (Exception ex)
        {
            // Log do erro mas continua processamento
            Console.WriteLine($"Erro ao obter arquivos: {ex.Message}");
        }

        // Ordena por caminho relativo
        return codeFiles.OrderBy(f => GetRelativePath(f.FullName, rootPath)).ToList();
    }

    private void GenerateIndex(StringBuilder content, List<FileInfo> codeFiles, string rootPath)
    {
        content.AppendLine("## ÍNDICE DE ARQUIVOS");
        content.AppendLine();
        content.AppendLine("Os seguintes arquivos estão incluídos neste documento:");
        content.AppendLine();

        var filesByExtension = codeFiles
            .GroupBy(f => Path.GetExtension(f.Name).ToLower())
            .OrderBy(g => g.Key);

        foreach (var group in filesByExtension)
        {
            content.AppendLine($"### {GetLanguageName(group.Key)} ({group.Key})");
            content.AppendLine();

            foreach (var file in group.OrderBy(f => GetRelativePath(f.FullName, rootPath)))
            {
                var relativePath = GetRelativePath(file.FullName, rootPath);
                var sizeKB = Math.Round(file.Length / 1024.0, 1);
                content.AppendLine($"- `{relativePath}` ({sizeKB} KB)");
            }
            content.AppendLine();
        }

        content.AppendLine($"**Total:** {codeFiles.Count} arquivos de código");
        content.AppendLine();
    }

    private void GenerateFileContents(StringBuilder content, List<FileInfo> codeFiles, string rootPath)
    {
        foreach (var file in codeFiles)
        {
            try
            {
                var relativePath = GetRelativePath(file.FullName, rootPath);
                var extension = Path.GetExtension(file.Name).ToLower();
                var languageCode = GetLanguageCode(extension);

                content.AppendLine($"## {relativePath}");
                content.AppendLine();
                content.AppendLine($"**Tipo:** {GetLanguageName(extension)}  ");
                content.AppendLine($"**Tamanho:** {Math.Round(file.Length / 1024.0, 1)} KB  ");
                content.AppendLine($"**Última Modificação:** {file.LastWriteTime:dd/MM/yyyy HH:mm:ss}");
                content.AppendLine();

                // Lê o conteúdo do arquivo
                string fileContent;
                try
                {
                    fileContent = File.ReadAllText(file.FullName, Encoding.UTF8);
                }
                catch (Exception)
                {
                    // Tenta com encoding padrão se UTF8 falhar
                    fileContent = File.ReadAllText(file.FullName);
                }

                // Remove caracteres de controle problemáticos
                fileContent = CleanFileContent(fileContent);

                content.AppendLine($"```{languageCode}");
                content.AppendLine(fileContent);
                content.AppendLine("```");
                content.AppendLine();
                content.AppendLine(new string('-', 80));
                content.AppendLine();
            }
            catch (Exception ex)
            {
                content.AppendLine($"**ERRO ao ler arquivo {file.Name}:** {ex.Message}");
                content.AppendLine();
                content.AppendLine(new string('-', 80));
                content.AppendLine();
            }
        }
    }

    private string GetRelativePath(string fullPath, string rootPath)
    {
        try
        {
            return Path.GetRelativePath(rootPath, fullPath).Replace('\\', '/');
        }
        catch
        {
            return Path.GetFileName(fullPath);
        }
    }

    private string GetLanguageCode(string extension)
    {
        return extension switch
        {
            ".cs" => "csharp",
            ".js" => "javascript",
            ".ts" => "typescript",
            ".py" => "python",
            ".java" => "java",
            ".cpp" or ".c" => "cpp",
            ".h" or ".hpp" => "cpp",
            ".php" => "php",
            ".rb" => "ruby",
            ".go" => "go",
            ".rs" => "rust",
            ".kt" => "kotlin",
            ".swift" => "swift",
            ".dart" => "dart",
            ".scala" => "scala",
            ".html" => "html",
            ".css" => "css",
            ".scss" => "scss",
            ".less" => "less",
            ".sql" => "sql",
            ".xml" => "xml",
            ".json" => "json",
            ".yaml" or ".yml" => "yaml",
            ".md" => "markdown",
            ".sh" => "bash",
            ".bat" => "batch",
            ".ps1" => "powershell",
            ".vue" => "vue",
            ".jsx" => "jsx",
            ".tsx" => "tsx",
            ".svelte" => "svelte",
            ".razor" => "razor",
            ".cshtml" => "html",
            ".vbhtml" => "html",
            _ => "text"
        };
    }

    private string GetLanguageName(string extension)
    {
        return extension switch
        {
            ".cs" => "C#",
            ".js" => "JavaScript",
            ".ts" => "TypeScript",
            ".py" => "Python",
            ".java" => "Java",
            ".cpp" => "C++",
            ".c" => "C",
            ".h" => "C Header",
            ".hpp" => "C++ Header",
            ".php" => "PHP",
            ".rb" => "Ruby",
            ".go" => "Go",
            ".rs" => "Rust",
            ".kt" => "Kotlin",
            ".swift" => "Swift",
            ".dart" => "Dart",
            ".scala" => "Scala",
            ".html" => "HTML",
            ".css" => "CSS",
            ".scss" => "SCSS",
            ".less" => "LESS",
            ".sql" => "SQL",
            ".xml" => "XML",
            ".json" => "JSON",
            ".yaml" or ".yml" => "YAML",
            ".md" => "Markdown",
            ".txt" => "Text",
            ".config" => "Configuration",
            ".ini" => "INI Configuration",
            ".toml" => "TOML Configuration",
            ".sh" => "Shell Script",
            ".bat" => "Batch Script",
            ".ps1" => "PowerShell",
            ".vue" => "Vue.js",
            ".jsx" => "JSX",
            ".tsx" => "TSX",
            ".svelte" => "Svelte",
            ".razor" => "Razor",
            ".cshtml" => "Razor HTML",
            ".vbhtml" => "VB.NET HTML",
            _ => extension.ToUpper().TrimStart('.')
        };
    }

    private string CleanFileContent(string content)
    {
        if (string.IsNullOrEmpty(content))
            return content;

        // Remove ou substitui caracteres de controle problemáticos
        var cleaned = new StringBuilder();

        foreach (char c in content)
        {
            if (c == '\r' || c == '\n' || c == '\t' || (c >= 32 && c < 127) || c >= 160)
            {
                cleaned.Append(c);
            }
            else
            {
                // Substitui caracteres de controle por espaço
                cleaned.Append(' ');
            }
        }

        return cleaned.ToString();
    }

    #endregion
}