using System.Xml.Linq;

namespace DesignProjectStructure.Helpers;

public class IgnoreFilter
{
    private static readonly string[] ignore = {
        ".git", ".vs", ".vscode", "bin", "obj", "packages",
        "node_modules", ".idea", "Debug", "Release",
        "Thumbs.db", ".DS_Store", "desktop.ini"
    };

    public static bool MustIgnore(string name)
    {
        if (name.StartsWith(".") && !name.EndsWith(".gitignore") && !name.EndsWith(".config"))
            return true;

        return Array.Exists(ignore, item =>
            string.Equals(item, name, StringComparison.OrdinalIgnoreCase));
    }
}

