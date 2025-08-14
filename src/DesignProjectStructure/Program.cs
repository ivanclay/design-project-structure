using DesignProjectStructure.Cli;
using DesignProjectStructure.Configuration;
using DesignProjectStructure.FileTypes;
using DesignProjectStructure.Helpers;
using DesignProjectStructure.Models;
using System.Text;

internal class Program
{
    public static void Main(string[] args)
    {
        // Verifica se foram passados argumentos da linha de comando
        if (args.Length > 0)
        {
            // Se há argumentos, executa diretamente (modo CLI)
            ExecuteDirectMode(args);
        }
        else
        {
            // Se não há argumentos, mostra o menu interativo
            var menuManager = new MenuManager();
            menuManager.Start();
        }
    }

    /// <summary>
    /// Executa a aplicação em modo direto (linha de comando)
    /// </summary>
    static void ExecuteDirectMode(string[] args)
    {
        Console.WriteLine("Design Project Structure - CLI Mode");
        Console.WriteLine("Loading configuration...");

        ExecuteStructureGeneration(args);
    }

    /// <summary>
    /// Executa a geração de estrutura (pode ser chamada pelo menu ou CLI)
    /// </summary>
    public static void ExecuteStructureGeneration(string[] args = null)
    {
        try
        {
            // Load configurations
            var config = ConfigurationManager.Instance.Config;
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
                outputFile = config.General.DefaultOutputPath;
            }
            else
            {
                outputFile = Path.Combine(rootPath, config.General.DefaultOutputPath);
            }

            // Process CLI arguments if provided
            if (args != null && args.Length > 0)
            {
                CliArguments cli = new CliArguments();
                rootPath = cli.UseCliArgumentsForRootPath(args, rootPath, structureItens);
                outputFile = cli.UseCliArgumentsForOutputFile(args, outputFile);

                bool isHelpCalled = cli.UseCliArgumentsForConfiguration(args, config);
                if (isHelpCalled)
                {
                    Console.WriteLine("Press any key....");
                    Console.ReadKey();
                    return;
                }
            }

            // Show initial information
            if (args == null || args.Length == 0)
            {
                // Modo menu - mostra informações na tela
                Console.WriteLine($"Configuration loaded! Animation: {(config.General.ShowConsoleAnimation ? "ON" : "OFF")}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            AnimatorStructureGenerator animator = new AnimatorStructureGenerator(structureItens);

            Console.CursorVisible = false;
            Console.Clear();

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

            // Salva nos formatos especificados na configuração usando os novos geradores
            SaveOutputFiles(outputFile, structureItens, rootPath);

            // Final message
            ConsoleRenderer.FinalMessage(outputFile);
        }
        catch (Exception ex)
        {
            ShowError($"Error during structure generation: {ex.Message}");
        }
        finally
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.CursorVisible = true;

            if (args == null || args.Length == 0)
            {
                // Modo menu - aguarda tecla para voltar
                Console.Write("Press any key to return to menu...");
                Console.ReadKey();
            }
            else
            {
                // Modo CLI - aguarda tecla para sair
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }

    /// <summary>
    /// Salva os arquivos de output usando os novos geradores
    /// </summary>
    static void SaveOutputFiles(string basePath, StructureItens structureItens, string rootPath)
    {
        var config = ConfigurationManager.Instance.Config;
        var directory = Path.GetDirectoryName(basePath) ?? "";
        var filename = Path.GetFileNameWithoutExtension(basePath);

        // Cria o diretório se não existir
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Processa cada formato configurado
        foreach (var format in config.Output.Formats)
        {
            try
            {
                // Verifica se o formato é suportado
                if (!OutputGeneratorFactory.IsSupported(format))
                {
                    Console.WriteLine($"⚠ Unsupported format: {format}");
                    Console.WriteLine($"  Supported formats: {string.Join(", ", OutputGeneratorFactory.GetSupportedFormats())}");
                    continue;
                }

                // Cria o gerador apropriado
                var generator = OutputGeneratorFactory.Create(format);

                // Gera o conteúdo
                var content = generator.Generate(structureItens, rootPath);

                // Define o arquivo de saída
                var outputFile = Path.Combine(directory, $"{filename}.{generator.GetFileExtension()}");

                // Salva o arquivo
                File.WriteAllText(outputFile, content, Encoding.UTF8);

                // Log de sucesso (comentado para não poluir a interface, mas pode ser habilitado)
                // Console.WriteLine($"✓ {generator.GetFormatName()} saved: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error saving format {format}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Mostra uma mensagem de erro
    /// </summary>
    static void ShowError(string message)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;

        var lines = message.Split('\n');
        int startY = Math.Max(1, (Console.WindowHeight - lines.Length) / 2);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            int startX = Math.Max(0, (Console.WindowWidth - line.Length) / 2);
            Console.SetCursorPosition(startX, startY + i);
            Console.Write(line);
        }

        Console.ResetColor();
        Console.WriteLine("\n\nPress any key to continue...");
        Console.ReadKey(true);
    }

}