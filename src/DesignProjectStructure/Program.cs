using System.Text;
using DesignProjectStructure.Helpers;
using DesignProjectStructure.Models;

Console.WriteLine("Design Project Structure");
Console.WriteLine("Waiting you press any key...");
Console.ReadKey();

// Configurations
string rootPath = "D:\\docs\\DesignProjectStructure";
string outputFile = "D:\\docs\\project-structure.md";

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
AnimatorStructureGenerator animator = new AnimatorStructureGenerator(structureItens);

// Allows the user to specify a different path
if (args.Length > 0)
{
    rootPath = args[0];
}

Console.CursorVisible = false;
Console.Clear();

try
{
    // Total item count first
    structureItens.TotalItems = StructureGenerator.CounterItens(rootPath);

    // Draw the initial interface
    ConsoleRenderer.DesignInterface();
    ConsoleRenderer.RenderHeader(rootPath, outputFile);

    // Prepare the structure
    structureItens.CompleteStructure.AppendLine($"Project Structure: {Path.GetFileName(rootPath)}");
    structureItens.CompleteStructure.AppendLine($"Path: {rootPath}");
    structureItens.CompleteStructure.AppendLine($"Generated in: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
    structureItens.CompleteStructure.AppendLine(new string('=', 60));
    structureItens.CompleteStructure.AppendLine();

    // Generates the structure with animation
    animator.GenerateStructureAnimated();

    // Ends the display
    Thread.Sleep(500);
    ConsoleRenderer.UpdateProgressBar(100);

    // Save to file
    FileWriter.SaveToFile(outputFile, structureItens.CompleteStructure.ToString());

    // Final message
    ConsoleRenderer.FinalMessage(outputFile);
}
catch (Exception ex)
{
    int startStatus = Console.WindowHeight - 12;
    Console.SetCursorPosition(3, startStatus + 8);
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write($"✗ ERRO: {ex.Message}");
    Console.ResetColor();
    Thread.Sleep(3000);
}

Console.SetCursorPosition(0, Console.WindowHeight - 1);
Console.CursorVisible = true;
Console.Write("Press any key to exit...");
Console.ReadKey();
