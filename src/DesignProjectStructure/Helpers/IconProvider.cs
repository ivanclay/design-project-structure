namespace DesignProjectStructure.Helpers;

public static class IconProvider
{
    /// <summary>
    /// Retorna ícones otimizados para exibição no console (texto simples)
    /// </summary>
    public static string GetConsoleIcon(string path)
    {
        // Se for um diretório
        if (Directory.Exists(path))
        {
            string folderName = Path.GetFileName(path).ToLower();

            return folderName switch
            {
                "controllers" => "[CTRL]",
                "models" => "[MODEL]",
                "views" => "[VIEW]",
                "data" => "[DATA]",
                "services" => "[SVC]",
                "repository" or "repositories" => "[REPO]",
                "config" or "configuration" => "[CFG]",
                "assets" => "[ASSET]",
                "images" or "img" => "[IMG]",
                "css" or "styles" => "[CSS]",
                "js" or "javascript" or "scripts" => "[JS]",
                "fonts" => "[FONT]",
                "docs" or "documentation" => "[DOCS]",
                "tests" or "test" => "[TEST]",
                "bin" => "[BIN]",
                "obj" => "[OBJ]",
                "wwwroot" => "[WWW]",
                "public" => "[PUB]",
                "src" or "source" => "[SRC]",
                "lib" or "libraries" => "[LIB]",
                "utils" or "utilities" => "[UTIL]",
                "helpers" => "[HELP]",
                "migrations" => "[MIG]",
                "logs" => "[LOG]",
                "temp" or "tmp" => "[TMP]",
                "backup" or "backups" => "[BAK]",
                _ => "[DIR]"
            };
        }

        // Se for um arquivo
        string extension = Path.GetExtension(path).ToLower();
        string fileName = Path.GetFileName(path).ToLower();

        // Ícones por nome específico do arquivo
        if (fileName.Contains("readme")) return "[README]";
        if (fileName.Contains("license")) return "[LIC]";
        if (fileName.Contains("changelog")) return "[CHANGE]";
        if (fileName.Contains("gitignore")) return "[GIT]";
        if (fileName.Contains("dockerfile")) return "[DOCKER]";
        if (fileName.Contains("makefile")) return "[MAKE]";

        return extension switch
        {
            ".cs" => "[C#]",
            ".csproj" => "[PROJ]",
            ".sln" => "[SLN]",
            ".config" => "[CFG]",
            ".dll" => "[DLL]",
            ".exe" => "[EXE]",
            ".html" or ".htm" => "[HTML]",
            ".css" => "[CSS]",
            ".js" => "[JS]",
            ".json" => "[JSON]",
            ".xml" => "[XML]",
            ".xaml" => "[XAML]",
            ".sql" => "[SQL]",
            ".db" or ".sqlite" => "[DB]",
            ".mdf" or ".ldf" => "[DB]",
            ".jpg" or ".jpeg" => "[JPG]",
            ".png" => "[PNG]",
            ".gif" => "[GIF]",
            ".svg" => "[SVG]",
            ".ico" => "[ICO]",
            ".bmp" => "[BMP]",
            ".txt" => "[TXT]",
            ".md" => "[MD]",
            ".pdf" => "[PDF]",
            ".doc" or ".docx" => "[DOC]",
            ".xls" or ".xlsx" => "[XLS]",
            ".ppt" or ".pptx" => "[PPT]",
            ".zip" => "[ZIP]",
            ".rar" => "[RAR]",
            ".7z" => "[7Z]",
            ".tar" => "[TAR]",
            ".yml" or ".yaml" => "[YML]",
            ".toml" => "[TOML]",
            ".ini" => "[INI]",
            ".conf" => "[CONF]",
            ".log" => "[LOG]",
            ".tmp" => "[TMP]",
            ".bak" => "[BAK]",
            ".py" => "[PY]",
            ".java" => "[JAVA]",
            ".cpp" or ".c" => "[C++]",
            ".php" => "[PHP]",
            ".rb" => "[RB]",
            ".go" => "[GO]",
            ".rs" => "[RUST]",
            ".ts" => "[TS]",
            "" when !string.IsNullOrEmpty(fileName) => "[FILE]",
            _ => "[FILE]"
        };
    }

    /// <summary>
    /// Retorna ícones Unicode para salvar no arquivo (mantém o comportamento original)
    /// </summary>
    public static string GetIcon(string path)
    {
        // Se for um diretório
        if (Directory.Exists(path))
        {
            string folderName = Path.GetFileName(path).ToLower();

            return folderName switch
            {
                "controllers" => "📁",
                "models" => "📁",
                "views" => "📁",
                "data" => "📊",
                "services" => "⚙️",
                "repository" or "repositories" => "🗄️",
                "config" or "configuration" => "⚙️",
                "assets" => "🎨",
                "images" or "img" => "🖼️",
                "css" or "styles" => "🎨",
                "js" or "javascript" or "scripts" => "📜",
                "fonts" => "🔤",
                "docs" or "documentation" => "📚",
                "tests" or "test" => "🧪",
                "bin" => "⚙️",
                "obj" => "⚙️",
                "wwwroot" => "🌐",
                "public" => "🌐",
                "src" or "source" => "📂",
                "lib" or "libraries" => "📚",
                "utils" or "utilities" => "🔧",
                "helpers" => "🔧",
                "migrations" => "🗃️",
                "logs" => "📋",
                "temp" or "tmp" => "🗂️",
                "backup" or "backups" => "💾",
                _ => "📁"
            };
        }

        // Se for um arquivo
        string extension = Path.GetExtension(path).ToLower();
        string fileName = Path.GetFileName(path).ToLower();

        // Ícones por nome específico do arquivo
        if (fileName.Contains("readme")) return "📖";
        if (fileName.Contains("license")) return "📄";
        if (fileName.Contains("changelog")) return "📝";
        if (fileName.Contains("gitignore")) return "🚫";
        if (fileName.Contains("dockerfile")) return "🐳";
        if (fileName.Contains("makefile")) return "⚡";

        return extension switch
        {
            ".cs" => "🔷",
            ".csproj" => "📋",
            ".sln" => "📂",
            ".config" => "⚙️",
            ".dll" => "⚙️",
            ".exe" => "⚡",
            ".html" or ".htm" => "🌐",
            ".css" => "🎨",
            ".js" => "📜",
            ".json" => "📄",
            ".xml" => "📄",
            ".xaml" => "🎨",
            ".sql" => "🗃️",
            ".db" or ".sqlite" => "🗄️",
            ".mdf" or ".ldf" => "🗄️",
            ".jpg" or ".jpeg" => "🖼️",
            ".png" => "🖼️",
            ".gif" => "🖼️",
            ".svg" => "🎨",
            ".ico" => "🖼️",
            ".bmp" => "🖼️",
            ".txt" => "📄",
            ".md" => "📖",
            ".pdf" => "📕",
            ".doc" or ".docx" => "📘",
            ".xls" or ".xlsx" => "📊",
            ".ppt" or ".pptx" => "📊",
            ".zip" => "📦",
            ".rar" => "📦",
            ".7z" => "📦",
            ".tar" => "📦",
            ".yml" or ".yaml" => "⚙️",
            ".toml" => "⚙️",
            ".ini" => "⚙️",
            ".conf" => "⚙️",
            ".log" => "📋",
            ".tmp" => "🗂️",
            ".bak" => "💾",
            ".py" => "🐍",
            ".java" => "☕",
            ".cpp" or ".c" => "⚡",
            ".php" => "🌐",
            ".rb" => "💎",
            ".go" => "🐹",
            ".rs" => "⚡",
            ".ts" => "📜",
            "" when !string.IsNullOrEmpty(fileName) => "📄",
            _ => "📄"
        };
    }
}