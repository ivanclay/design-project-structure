namespace DesignProjectStructure.Helpers;

public class ProjectTypeDetector
{
    public class ProjectInfo
    {
        public string Type { get; set; } = "Unknown";
        public string Framework { get; set; } = "";
        public string Language { get; set; } = "";
        public string Description { get; set; } = "";
        public string Icon { get; set; } = "[PROJ]";
        public List<string> DetectedFiles { get; set; } = new List<string>();
    }

    public static ProjectInfo DetectProjectType(string rootPath)
    {
        var projectInfo = new ProjectInfo();

        try
        {
            var allFiles = GetAllFiles(rootPath);
            var rootFiles = Directory.GetFiles(rootPath).Select(Path.GetFileName).ToList();
            var allDirectories = GetAllDirectories(rootPath);

            // .NET Projects
            if (DetectDotNetProject(allFiles, rootFiles, allDirectories, projectInfo))
                return projectInfo;

            // Node.js Projects
            if (DetectNodeJsProject(rootFiles, allFiles, projectInfo))
                return projectInfo;

            // Python Projects
            if (DetectPythonProject(rootFiles, allFiles, allDirectories, projectInfo))
                return projectInfo;

            // Java Projects
            if (DetectJavaProject(rootFiles, allFiles, allDirectories, projectInfo))
                return projectInfo;

            // PHP Projects
            if (DetectPhpProject(rootFiles, allFiles, allDirectories, projectInfo))
                return projectInfo;

            // Ruby Projects
            if (DetectRubyProject(rootFiles, allFiles, allDirectories, projectInfo))
                return projectInfo;

            // Go Projects
            if (DetectGoProject(rootFiles, allFiles, projectInfo))
                return projectInfo;

            // Rust Projects
            if (DetectRustProject(rootFiles, allFiles, projectInfo))
                return projectInfo;

            // Frontend Projects
            if (DetectFrontendProject(rootFiles, allFiles, allDirectories, projectInfo))
                return projectInfo;

            // Generic detection based on most common files
            DetectGenericProject(allFiles, projectInfo);
        }
        catch (Exception)
        {
            // Em caso de erro, retorna projeto desconhecido
        }

        return projectInfo;
    }

    private static bool DetectDotNetProject(List<string> allFiles, List<string> rootFiles, List<string> allDirectories, ProjectInfo projectInfo)
    {
        var csprojFiles = allFiles.Where(f => f.EndsWith(".csproj")).ToList();
        var slnFiles = rootFiles.Where(f => f.EndsWith(".sln")).ToList();
        var csFiles = allFiles.Where(f => f.EndsWith(".cs")).ToList();

        if (csprojFiles.Any() || slnFiles.Any() || csFiles.Count > 3)
        {
            projectInfo.Language = "C#";
            projectInfo.Framework = ".NET";
            projectInfo.Icon = "[.NET]";
            projectInfo.DetectedFiles.AddRange(csprojFiles.Take(3));
            projectInfo.DetectedFiles.AddRange(slnFiles);

            // Detect specific .NET project types
            var programCs = allFiles.FirstOrDefault(f => f.EndsWith("Program.cs"));
            var startupCs = allFiles.FirstOrDefault(f => f.EndsWith("Startup.cs"));
            var controllersDir = allDirectories.Any(d => d.ToLower().Contains("controllers"));
            var wwwrootDir = allDirectories.Any(d => d.ToLower().Contains("wwwroot"));

            if (controllersDir || startupCs != null || wwwrootDir)
            {
                projectInfo.Type = "ASP.NET Web API / MVC";
                projectInfo.Description = "ASP.NET Core Web Application";
            }
            else if (programCs != null)
            {
                projectInfo.Type = ".NET Console Application";
                projectInfo.Description = ".NET Console Application";
            }
            else
            {
                projectInfo.Type = ".NET Library/Application";
                projectInfo.Description = ".NET Class Library or Application";
            }

            return true;
        }

        return false;
    }

    private static bool DetectNodeJsProject(List<string> rootFiles, List<string> allFiles, ProjectInfo projectInfo)
    {
        if (rootFiles.Contains("package.json"))
        {
            projectInfo.Language = "JavaScript/TypeScript";
            projectInfo.Framework = "Node.js";
            projectInfo.Icon = "[NODE]";
            projectInfo.DetectedFiles.Add("package.json");

            var tsFiles = allFiles.Where(f => f.EndsWith(".ts")).Count();
            var jsFiles = allFiles.Where(f => f.EndsWith(".js")).Count();

            if (tsFiles > jsFiles)
                projectInfo.Language = "TypeScript";

            if (rootFiles.Contains("next.config.js") || allFiles.Any(f => f.Contains("next")))
            {
                projectInfo.Type = "Next.js Application";
                projectInfo.Description = "Next.js React Framework";
            }
            else if (rootFiles.Contains("angular.json") || allFiles.Any(f => f.Contains("angular")))
            {
                projectInfo.Type = "Angular Application";
                projectInfo.Description = "Angular Frontend Framework";
            }
            else if (allFiles.Any(f => f.Contains("react")) || rootFiles.Contains("yarn.lock"))
            {
                projectInfo.Type = "React Application";
                projectInfo.Description = "React Frontend Application";
            }
            else if (allFiles.Any(f => f.Contains("express")))
            {
                projectInfo.Type = "Express.js API";
                projectInfo.Description = "Express.js Backend API";
            }
            else
            {
                projectInfo.Type = "Node.js Application";
                projectInfo.Description = "Node.js Application";
            }

            return true;
        }

        return false;
    }

    private static bool DetectPythonProject(List<string> rootFiles, List<string> allFiles, List<string> allDirectories, ProjectInfo projectInfo)
    {
        var pythonFiles = allFiles.Where(f => f.EndsWith(".py")).Count();

        if (pythonFiles > 0 || rootFiles.Contains("requirements.txt") || rootFiles.Contains("setup.py") || rootFiles.Contains("pyproject.toml"))
        {
            projectInfo.Language = "Python";
            projectInfo.Framework = "Python";
            projectInfo.Icon = "[PY]";

            if (rootFiles.Contains("requirements.txt"))
                projectInfo.DetectedFiles.Add("requirements.txt");
            if (rootFiles.Contains("setup.py"))
                projectInfo.DetectedFiles.Add("setup.py");

            if (allFiles.Any(f => f.Contains("django")) || rootFiles.Contains("manage.py"))
            {
                projectInfo.Type = "Django Web Application";
                projectInfo.Description = "Django Web Framework";
            }
            else if (allFiles.Any(f => f.Contains("flask")))
            {
                projectInfo.Type = "Flask Web Application";
                projectInfo.Description = "Flask Micro Web Framework";
            }
            else if (allFiles.Any(f => f.Contains("fastapi")))
            {
                projectInfo.Type = "FastAPI Application";
                projectInfo.Description = "FastAPI Modern Web Framework";
            }
            else
            {
                projectInfo.Type = "Python Application";
                projectInfo.Description = "Python Application or Script";
            }

            return true;
        }

        return false;
    }

    private static bool DetectJavaProject(List<string> rootFiles, List<string> allFiles, List<string> allDirectories, ProjectInfo projectInfo)
    {
        var javaFiles = allFiles.Where(f => f.EndsWith(".java")).Count();

        if (javaFiles > 0 || rootFiles.Contains("pom.xml") || rootFiles.Contains("build.gradle") || rootFiles.Contains("build.xml"))
        {
            projectInfo.Language = "Java";
            projectInfo.Framework = "Java";
            projectInfo.Icon = "[JAVA]";

            if (rootFiles.Contains("pom.xml"))
            {
                projectInfo.Framework = "Maven";
                projectInfo.DetectedFiles.Add("pom.xml");
            }
            else if (rootFiles.Contains("build.gradle"))
            {
                projectInfo.Framework = "Gradle";
                projectInfo.DetectedFiles.Add("build.gradle");
            }

            if (allFiles.Any(f => f.Contains("spring")))
            {
                projectInfo.Type = "Spring Boot Application";
                projectInfo.Description = "Spring Boot Java Framework";
            }
            else
            {
                projectInfo.Type = "Java Application";
                projectInfo.Description = "Java Application";
            }

            return true;
        }

        return false;
    }

    private static bool DetectPhpProject(List<string> rootFiles, List<string> allFiles, List<string> allDirectories, ProjectInfo projectInfo)
    {
        var phpFiles = allFiles.Where(f => f.EndsWith(".php")).Count();

        if (phpFiles > 0 || rootFiles.Contains("composer.json"))
        {
            projectInfo.Language = "PHP";
            projectInfo.Framework = "PHP";
            projectInfo.Icon = "[PHP]";

            if (rootFiles.Contains("composer.json"))
                projectInfo.DetectedFiles.Add("composer.json");

            if (allFiles.Any(f => f.Contains("laravel")) || allDirectories.Any(d => d.ToLower().Contains("artisan")))
            {
                projectInfo.Type = "Laravel Application";
                projectInfo.Description = "Laravel PHP Framework";
            }
            else if (allFiles.Any(f => f.Contains("symfony")))
            {
                projectInfo.Type = "Symfony Application";
                projectInfo.Description = "Symfony PHP Framework";
            }
            else
            {
                projectInfo.Type = "PHP Application";
                projectInfo.Description = "PHP Web Application";
            }

            return true;
        }

        return false;
    }

    private static bool DetectRubyProject(List<string> rootFiles, List<string> allFiles, List<string> allDirectories, ProjectInfo projectInfo)
    {
        var rubyFiles = allFiles.Where(f => f.EndsWith(".rb")).Count();

        if (rubyFiles > 0 || rootFiles.Contains("Gemfile") || rootFiles.Contains("Rakefile"))
        {
            projectInfo.Language = "Ruby";
            projectInfo.Framework = "Ruby";
            projectInfo.Icon = "[RUBY]";

            if (rootFiles.Contains("Gemfile"))
                projectInfo.DetectedFiles.Add("Gemfile");

            if (allFiles.Any(f => f.Contains("rails")) || allDirectories.Any(d => d.ToLower().Contains("config")))
            {
                projectInfo.Type = "Ruby on Rails Application";
                projectInfo.Description = "Ruby on Rails Framework";
            }
            else
            {
                projectInfo.Type = "Ruby Application";
                projectInfo.Description = "Ruby Application or Script";
            }

            return true;
        }

        return false;
    }

    private static bool DetectGoProject(List<string> rootFiles, List<string> allFiles, ProjectInfo projectInfo)
    {
        var goFiles = allFiles.Where(f => f.EndsWith(".go")).Count();

        if (goFiles > 0 || rootFiles.Contains("go.mod") || rootFiles.Contains("go.sum"))
        {
            projectInfo.Language = "Go";
            projectInfo.Framework = "Go";
            projectInfo.Type = "Go Application";
            projectInfo.Description = "Go Application or Service";
            projectInfo.Icon = "[GO]";

            if (rootFiles.Contains("go.mod"))
                projectInfo.DetectedFiles.Add("go.mod");

            return true;
        }

        return false;
    }

    private static bool DetectRustProject(List<string> rootFiles, List<string> allFiles, ProjectInfo projectInfo)
    {
        var rustFiles = allFiles.Where(f => f.EndsWith(".rs")).Count();

        if (rustFiles > 0 || rootFiles.Contains("Cargo.toml") || rootFiles.Contains("Cargo.lock"))
        {
            projectInfo.Language = "Rust";
            projectInfo.Framework = "Rust";
            projectInfo.Type = "Rust Application";
            projectInfo.Description = "Rust Application or Library";
            projectInfo.Icon = "[RUST]";

            if (rootFiles.Contains("Cargo.toml"))
                projectInfo.DetectedFiles.Add("Cargo.toml");

            return true;
        }

        return false;
    }

    private static bool DetectFrontendProject(List<string> rootFiles, List<string> allFiles, List<string> allDirectories, ProjectInfo projectInfo)
    {
        var htmlFiles = allFiles.Where(f => f.EndsWith(".html")).Count();
        var cssFiles = allFiles.Where(f => f.EndsWith(".css")).Count();
        var jsFiles = allFiles.Where(f => f.EndsWith(".js")).Count();

        if (htmlFiles > 0 && (cssFiles > 0 || jsFiles > 0) && !rootFiles.Contains("package.json"))
        {
            projectInfo.Language = "HTML/CSS/JavaScript";
            projectInfo.Framework = "Frontend";
            projectInfo.Type = "Static Website";
            projectInfo.Description = "Static HTML/CSS/JS Website";
            projectInfo.Icon = "[WEB]";

            return true;
        }

        return false;
    }

    private static void DetectGenericProject(List<string> allFiles, ProjectInfo projectInfo)
    {
        var extensions = allFiles.Select(f => Path.GetExtension(f).ToLower())
                                .Where(ext => !string.IsNullOrEmpty(ext))
                                .GroupBy(ext => ext)
                                .OrderByDescending(g => g.Count())
                                .Take(3)
                                .Select(g => g.Key)
                                .ToList();

        if (extensions.Any())
        {
            projectInfo.Type = "Mixed Project";
            projectInfo.Description = $"Project with: {string.Join(", ", extensions)}";
            projectInfo.Language = string.Join(", ", extensions);
        }
    }

    private static List<string> GetAllFiles(string rootPath)
    {
        var files = new List<string>();
        try
        {
            files.AddRange(Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories)
                          .Where(f => !IgnoreFilter.MustIgnore(Path.GetFileName(f)))
                          .Take(500)); // Limita para performance
        }
        catch { }
        return files;
    }

    private static List<string> GetAllDirectories(string rootPath)
    {
        var directories = new List<string>();
        try
        {
            directories.AddRange(Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories)
                               .Where(d => !IgnoreFilter.MustIgnore(Path.GetFileName(d)))
                               .Take(200)); // Limita para performance
        }
        catch { }
        return directories;
    }
}