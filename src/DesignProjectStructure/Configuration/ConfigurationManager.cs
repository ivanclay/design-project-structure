namespace DesignProjectStructure.Configuration;

using System.Text.Json;

public class ConfigurationManager
{
    private static ConfigurationManager? _instance;
    private Configuration _config;
    private readonly string _configPath;

    public static ConfigurationManager Instance => _instance ??= new ConfigurationManager();

    private ConfigurationManager()
    {
        _configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        _config = LoadConfiguration();
    }

    public Configuration Config => _config;

    private Configuration LoadConfiguration()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                var jsonString = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<Configuration>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                });
                return config ?? CreateDefaultConfiguration();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar configuração: {ex.Message}");
            Console.WriteLine("Usando configuração padrão...");
        }

        // Se não existe ou houve erro, cria configuração padrão
        var defaultConfig = CreateDefaultConfiguration();
        SaveConfiguration(defaultConfig);
        return defaultConfig;
    }

    public void SaveConfiguration(Configuration? config = null)
    {
        try
        {
            var configToSave = config ?? _config;
            var jsonString = JsonSerializer.Serialize(configToSave, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            File.WriteAllText(_configPath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar configuração: {ex.Message}");
        }
    }

    public void ReloadConfiguration()
    {
        _config = LoadConfiguration();
    }

    private Configuration CreateDefaultConfiguration()
    {
        return new Configuration
        {
            General = new GeneralSettings
            {
                DefaultOutputPath = "project-structure.md", // Mudança: path mais simples
                ShowConsoleAnimation = true,
                AnimationDelay = 50,
                MaxDepth = -1,
                IncludeHiddenFiles = false
            },
            Filters = new FilterSettings
            {
                IgnoreFolders = new List<string>
                {
                    ".git", ".vs", ".vscode", "bin", "obj", "packages",
                    "node_modules", ".idea", "Debug", "Release", "target",
                    "__pycache__", ".pytest_cache", "dist", "build"
                },
                IgnoreFiles = new List<string>
                {
                    "Thumbs.db", ".DS_Store", "desktop.ini", "*.tmp", "*.log"
                },
                IgnoreExtensions = new List<string>
                {
                    ".exe", ".dll", ".pdb", ".cache", ".suo", ".user"
                },
                CustomIgnorePatterns = new List<string>
                {
                    "**/node_modules/**", "**/bin/Debug/**", "**/obj/**"
                }
            },
            Output = new OutputSettings
            {
                Formats = new List<string> { "markdown", "json" },
                IncludeStats = true,
                IncludeProjectInfo = true,
                IncludeTimestamp = true,
                MaxFileNameLength = 50
            },
            Display = new DisplaySettings
            {
                ConsoleIcons = new Dictionary<string, string>
                {
                    ["folder"] = "[DIR]",
                    ["file"] = "[FILE]",
                    ["csharp"] = "[C#]",
                    ["javascript"] = "[JS]",
                    ["typescript"] = "[TS]",
                    ["python"] = "[PY]",
                    ["java"] = "[JAVA]",
                    ["html"] = "[HTML]",
                    ["css"] = "[CSS]",
                    ["json"] = "[JSON]",
                    ["xml"] = "[XML]",
                    ["markdown"] = "[MD]",
                    ["image"] = "[IMG]",
                    ["config"] = "[CFG]",
                    ["database"] = "[DB]",
                    ["archive"] = "[ZIP]"
                },
                UnicodeIcons = new Dictionary<string, string>
                {
                    ["folder"] = "📁",
                    ["file"] = "📄",
                    ["csharp"] = "🔷",
                    ["javascript"] = "📜",
                    ["typescript"] = "📜",
                    ["python"] = "🐍",
                    ["java"] = "☕",
                    ["html"] = "🌐",
                    ["css"] = "🎨",
                    ["json"] = "📄",
                    ["xml"] = "📄",
                    ["markdown"] = "📖",
                    ["image"] = "🖼️",
                    ["config"] = "⚙️",
                    ["database"] = "🗄️",
                    ["archive"] = "📦"
                }
            },
            ProjectDetection = new ProjectDetectionSettings
            {
                EnableAdvancedDetection = true,
                CustomPatterns = new Dictionary<string, CustomPattern>()
            },
            Statistics = new StatisticsSettings
            {
                CalculateFileSize = true,
                CountCodeLines = true,
                AnalyzeComplexity = false,
                IncludeDependencies = true
            }
        };
    }
}

// Classes de configuração
public class Configuration
{
    public GeneralSettings General { get; set; } = new();
    public FilterSettings Filters { get; set; } = new();
    public OutputSettings Output { get; set; } = new();
    public DisplaySettings Display { get; set; } = new();
    public ProjectDetectionSettings ProjectDetection { get; set; } = new();
    public StatisticsSettings Statistics { get; set; } = new();
}

public class GeneralSettings
{
    public string DefaultOutputPath { get; set; } = "docs/project-structure.md";
    public bool ShowConsoleAnimation { get; set; } = true;
    public int AnimationDelay { get; set; } = 50;
    public int MaxDepth { get; set; } = -1; // -1 = unlimited
    public bool IncludeHiddenFiles { get; set; } = false;
}

public class FilterSettings
{
    public List<string> IgnoreFolders { get; set; } = new();
    public List<string> IgnoreFiles { get; set; } = new();
    public List<string> IgnoreExtensions { get; set; } = new();
    public List<string> CustomIgnorePatterns { get; set; } = new();
}

public class OutputSettings
{
    public List<string> Formats { get; set; } = new();
    public bool IncludeStats { get; set; } = true;
    public bool IncludeProjectInfo { get; set; } = true;
    public bool IncludeTimestamp { get; set; } = true;
    public int MaxFileNameLength { get; set; } = 50;
}

public class DisplaySettings
{
    public Dictionary<string, string> ConsoleIcons { get; set; } = new();
    public Dictionary<string, string> UnicodeIcons { get; set; } = new();
}

public class ProjectDetectionSettings
{
    public bool EnableAdvancedDetection { get; set; } = true;
    public Dictionary<string, CustomPattern> CustomPatterns { get; set; } = new();
}

public class CustomPattern
{
    public List<string> Files { get; set; } = new();
    public List<string> Folders { get; set; } = new();
    public string Description { get; set; } = "";
}

public class StatisticsSettings
{
    public bool CalculateFileSize { get; set; } = true;
    public bool CountCodeLines { get; set; } = true;
    public bool AnalyzeComplexity { get; set; } = false;
    public bool IncludeDependencies { get; set; } = true;
}