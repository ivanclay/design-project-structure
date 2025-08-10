using DesignProjectStructure.Models;

namespace DesignProjectStructure.Cli;

internal class CliArguments
{
    internal string UseCliArgumentsForOutputFile(string[] args, string outputFile)
    {
        if (args.Length > 1)
        {
            outputFile = args[1];
        }

        return outputFile;
    }
    internal string UseCliArgumentsForRootPath(string[] args, string rootPath, StructureItens structureItens)
    {
        if (args.Length > 0)
        {
            rootPath = args[0];
            structureItens.Path = rootPath;
        }

        return rootPath;
    }

    private void ShowHelp()
    {
        Console.Clear();
        Console.WriteLine(@"
Design Project Structure - Generates project structure visualization

USAGE:
    DesignProjectStructure [path] [output] [options]

ARGUMENTS:
    path        Project Path (default: current directory)
    output      Output File (default: configuration)

OPTIONS:
    --no-animation    Disables console animation
    --config <file>   Use custom configuration file
    -h, --help        Show this help message

EXAMPLES:
    DesignProjectStructure
    DesignProjectStructure C:\MyProject
    DesignProjectStructure C:\MyProject docs\output-structure.md
    DesignProjectStructure --no-animation

CONFIGURATION:
    The appsettings.json file allows you to customize:
    - File and folder filters
    - Output formats
    - Icons and visualization
    - Analysis settings
");
    }

    internal bool UseCliArgumentsForConfiguration(string[] args, Configuration.Configuration config)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--no-animation")
            {
                config.General.ShowConsoleAnimation = false;
            }
            else if (args[i] == "--config" && i + 1 < args.Length)
            {
                // Carrega configuração customizada (implementar se necessário)
                Console.WriteLine($"Custom config not implemented yet: {args[i + 1]}");
            }
            else if (args[i] == "--help" || args[i] == "-h")
            {
                ShowHelp();
                return true;
            }
        }

        return false;
    }
}
