using DesignProjectStructure.Cli;
using DesignProjectStructure.Configuration;
using DesignProjectStructure.FileTypes;
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
    // If the path in the configuration is absolute, use it
    outputFile = config.General.DefaultOutputPath;
}
else
{
    // If relative, match the project directory (not the execution directory)
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

    // Save in the formats specified in the configuration using the new generators
    SaveOutputFiles(outputFile, structureItens, rootPath);

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

/// <summary>
/// Saves the output files using the new generators
/// </summary>
static void SaveOutputFiles(string basePath, StructureItens structureItens, string rootPath)
{
    var config = ConfigurationManager.Instance.Config;
    var directory = Path.GetDirectoryName(basePath) ?? "";
    var filename = Path.GetFileNameWithoutExtension(basePath);

    // Create the directory if it does not exist
    if (!Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory);
    }

    // Process each configured format
    foreach (var format in config.Output.Formats)
    {
        try
        {
            // Check if the format is supported
            if (!OutputGeneratorFactory.IsSupported(format))
            {
                Console.WriteLine($"⚠ Unsupported format: {format}");
                Console.WriteLine($"  Supported formats: {string.Join(", ", OutputGeneratorFactory.GetSupportedFormats())}");
                continue;
            }

            // Create the appropriate generator
            var generator = OutputGeneratorFactory.Create(format);

            // Generates the content
            var content = generator.Generate(structureItens, rootPath);

            // Define o arquivo de saída
            var outputFile = Path.Combine(directory, $"{filename}.{generator.GetFileExtension()}");

            // Save file
            File.WriteAllText(outputFile, content, Encoding.UTF8);

            // Success log (commented out to avoid cluttering the interface, but can be enabled)
            // Console.WriteLine($"✓ {generator.GetFormatName()} saved: {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error saving format {format}: {ex.Message}");
        }
    }
}