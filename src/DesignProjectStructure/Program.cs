using DesignProjectStructure.Cli;
using DesignProjectStructure.Configuration;
using DesignProjectStructure.Helpers;
using DesignProjectStructure.Models;
using System.Text;

Console.WriteLine("Design Project Structure");
Console.WriteLine("Loading configuration...");

// Load configurations
var config = ConfigurationManager.Instance.Config;
// Default root path configuration file
string rootPath = Directory.GetCurrentDirectory();

// Global variables for control
StructureItens structureItens = new StructureItens
{
    Path = rootPath,
    Prefix = "",
    IsLast = true,
    FolderCounter = 0,
    FileCounter = 0,
    ProcessedItems = 0,
    CompleteStructure = new StringBuilder(),
    VisualStructure = new List<string>(),
    TotalItems = 0
};

// Build output path
string outputFile;
if (Path.IsPathRooted(config.General.DefaultOutputPath))
{
    // Se o caminho na configuração for absoluto, usa ele
    outputFile = config.General.DefaultOutputPath;
}
else
{
    // Se for relativo, combina com o diretório do projeto (não o de execução)
    outputFile = Path.Combine(rootPath, config.General.DefaultOutputPath);
}

CliArguments cli = new CliArguments();
// Allows the user to specify a different path
rootPath = cli.UseCliArgumentsForRootPath(args, rootPath, structureItens);

// Allows the user to specify a different output file
outputFile = cli.UseCliArgumentsForOutputFile(args, outputFile);

bool isHelpCalled = cli.UseCliArgumentsForConfiguration(args, config);

if (isHelpCalled)
{
    Console.WriteLine("Press any key....");
    Console.ReadKey();
    return;
}

Console.WriteLine($"Configuration loaded! Animation: {(config.General.ShowConsoleAnimation ? "ON" : "OFF")}");
Console.WriteLine("Press any key to continue...");
Console.ReadKey();

AnimatorStructureGenerator animator = new AnimatorStructureGenerator(structureItens);

Console.CursorVisible = false;
Console.Clear();

try
{
    // Total item count first
    Console.WriteLine("Counting files and folders...");
    structureItens.TotalItems = StructureGenerator.CounterItens(rootPath);

    if (!config.General.ShowConsoleAnimation)
    {
        Console.Clear();
    }

    // Design the initial interface
    ConsoleRenderer.DesignInterface();
    ConsoleRenderer.RenderHeader(rootPath, outputFile);

    // Prepare structure
    structureItens.CompleteStructure.AppendLine($"Project Structure: {Path.GetFileName(rootPath)}");
    structureItens.CompleteStructure.AppendLine($"Path: {rootPath}");

    if (config.Output.IncludeTimestamp)
    {
        structureItens.CompleteStructure.AppendLine($"Generated in: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
    }

    structureItens.CompleteStructure.AppendLine(new string('=', 60));
    structureItens.CompleteStructure.AppendLine();

    // Generates structure (with or without animation based on the configuration)
    animator.GenerateStructureAnimated();

    // If animation is disabled, update final status
    if (!config.General.ShowConsoleAnimation)
    {
        StructureGenerator.UpdateFinalStatus(
            structureItens.FolderCounter,
            structureItens.FileCounter,
            structureItens.ProcessedItems,
            structureItens.TotalItems);
    }
    else
    {
        Thread.Sleep(500);
    }

    ConsoleRenderer.UpdateProgressBar(100);

    // Salva nos formatos especificados na configuração
    //Console.WriteLine($"\nSaving files to: {outputFile}");
    //Console.WriteLine($"Formats: {string.Join(", ", config.Output.Formats)}");

    foreach (var format in config.Output.Formats)
    {
        SaveInFormat(format, outputFile, structureItens, rootPath);
    }

    // Final message
    ConsoleRenderer.FinalMessage(outputFile);
}
catch (Exception ex)
{
    int startStatus = Console.WindowHeight - 12;
    Console.SetCursorPosition(3, startStatus + 8);
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write($"✗ ERROR: {ex.Message}");
    Console.ResetColor();
    Thread.Sleep(3000);
}

Console.SetCursorPosition(0, Console.WindowHeight - 1);
Console.CursorVisible = true;
Console.Write("Press any key to exit...");
Console.ReadKey();

static void SaveInFormat(string format, string basePath, StructureItens structureItens, string rootPath)
{
    try
    {
        var config = ConfigurationManager.Instance.Config;
        var directory = Path.GetDirectoryName(basePath) ?? "";
        var filename = Path.GetFileNameWithoutExtension(basePath);

        // Cria o diretório se não existir
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string outputFile;

        switch (format.ToLower())
        {
            case "markdown":
            case "md":
                outputFile = Path.Combine(directory, $"{filename}.md");
                FileWriter.SaveToFile(outputFile, structureItens.CompleteStructure.ToString());
                //Console.WriteLine($"✓ Markdown saved: {outputFile}");
                break;

            case "json":
                outputFile = Path.Combine(directory, $"{filename}.json");
                var jsonContent = GenerateJsonOutput(structureItens, rootPath);
                File.WriteAllText(outputFile, jsonContent);
                //Console.WriteLine($"✓ JSON saved: {outputFile}");
                break;

            case "html":
                outputFile = Path.Combine(directory, $"{filename}.html");
                var htmlContent = GenerateHtmlOutput(structureItens, rootPath);
                File.WriteAllText(outputFile, htmlContent);
                //Console.WriteLine($"✓ HTML saved: {outputFile}");
                break;

            default:
                Console.WriteLine($"⚠ Unsupported format: {format}");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Error saving format {format}: {ex.Message}");
    }
}

static string GenerateJsonOutput(StructureItens structureItens, string rootPath)
{
    // Implementação básica - pode ser expandida
    var output = new
    {
        projectName = Path.GetFileName(rootPath),
        path = rootPath,
        generatedAt = DateTime.Now,
        statistics = new
        {
            totalFolders = structureItens.FolderCounter,
            totalFiles = structureItens.FileCounter,
            totalItems = structureItens.ProcessedItems
        },
        structure = structureItens.VisualStructure
    };

    return System.Text.Json.JsonSerializer.Serialize(output, new System.Text.Json.JsonSerializerOptions
    {
        WriteIndented = true
    });
}

static string GenerateHtmlOutput(StructureItens structureItens, string rootPath)
{
    // Converte a estrutura completa (que tem ícones Unicode) para HTML
    var htmlStructure = structureItens.CompleteStructure.ToString()
        .Replace("&", "&amp;")
        .Replace("<", "&lt;")
        .Replace(">", "&gt;");

    // Remove as linhas de cabeçalho (Project Structure:, Path:, Generated in:, ====)
    var lines = htmlStructure.Split('\n');
    var structureLines = lines.Skip(4).Where(line => !string.IsNullOrWhiteSpace(line));
    var cleanStructure = string.Join("\n", structureLines);

    var html = $@"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Project Structure - {Path.GetFileName(rootPath)}</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
        }}
        
        .container {{
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 15px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        
        .header {{
            background: linear-gradient(135deg, #2c3e50 0%, #34495e 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        
        .header h1 {{
            font-size: 2.5em;
            margin-bottom: 10px;
            text-shadow: 0 2px 4px rgba(0,0,0,0.3);
        }}
        
        .header .subtitle {{
            font-size: 1.1em;
            opacity: 0.9;
            margin: 5px 0;
        }}
        
        .stats {{
            background: #ecf0f1;
            padding: 20px 30px;
            display: flex;
            justify-content: space-around;
            flex-wrap: wrap;
            border-bottom: 1px solid #bdc3c7;
        }}
        
        .stat-item {{
            text-align: center;
            margin: 10px;
        }}
        
        .stat-number {{
            font-size: 2em;
            font-weight: bold;
            color: #2c3e50;
            display: block;
        }}
        
        .stat-label {{
            color: #7f8c8d;
            font-size: 0.9em;
            text-transform: uppercase;
            letter-spacing: 1px;
        }}
        
        .structure {{
            padding: 30px;
            background: #fafafa;
        }}
        
        .structure h2 {{
            color: #2c3e50;
            margin-bottom: 20px;
            font-size: 1.5em;
            border-bottom: 2px solid #3498db;
            padding-bottom: 10px;
        }}
        
        .tree {{
            background: #2c3e50;
            color: #ecf0f1;
            padding: 25px;
            border-radius: 10px;
            font-family: 'Courier New', 'Monaco', 'Menlo', monospace;
            font-size: 14px;
            line-height: 1.6;
            overflow-x: auto;
            box-shadow: inset 0 2px 10px rgba(0,0,0,0.1);
        }}
        
        .footer {{
            background: #34495e;
            color: white;
            padding: 20px;
            text-align: center;
            font-size: 0.9em;
        }}
        
        .footer a {{
            color: #3498db;
            text-decoration: none;
        }}
        
        .footer a:hover {{
            text-decoration: underline;
        }}
        
        @media (max-width: 768px) {{
            body {{ padding: 10px; }}
            .header {{ padding: 20px; }}
            .header h1 {{ font-size: 2em; }}
            .stats {{ padding: 15px; }}
            .structure {{ padding: 20px; }}
            .tree {{ padding: 15px; font-size: 12px; }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <header class=""header"">
            <h1>📁 Project Structure</h1>
            <div class=""subtitle"">🎯 {Path.GetFileName(rootPath)}</div>
            <div class=""subtitle"">📍 {rootPath}</div>
            <div class=""subtitle"">🕒 Generated on {DateTime.Now:dd/MM/yyyy HH:mm:ss}</div>
        </header>
        
        <section class=""stats"">
            <div class=""stat-item"">
                <span class=""stat-number"">{structureItens.FolderCounter}</span>
                <span class=""stat-label"">📁 Folders</span>
            </div>
            <div class=""stat-item"">
                <span class=""stat-number"">{structureItens.FileCounter}</span>
                <span class=""stat-label"">📄 Files</span>
            </div>
            <div class=""stat-item"">
                <span class=""stat-number"">{structureItens.ProcessedItems}</span>
                <span class=""stat-label"">✅ Total Items</span>
            </div>
        </section>
        
        <section class=""structure"">
            <h2>🌳 File Tree Structure</h2>
            <div class=""tree""><pre>{cleanStructure}</pre></div>
        </section>
        
        <footer class=""footer"">
            <p>🚀 Generated by <strong>Design Project Structure</strong> | 
               ⚡ Processing completed in milliseconds | 
               💻 <a href=""https://github.com/yourusername/design-project-structure"" target=""_blank"">View on GitHub</a>
            </p>
        </footer>
    </div>
</body>
</html>";

    return html;
}

