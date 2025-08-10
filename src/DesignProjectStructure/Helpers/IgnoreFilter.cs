using DesignProjectStructure.Configuration;
using Microsoft.Extensions.FileSystemGlobbing;

namespace DesignProjectStructure.Helpers;

public static class IgnoreFilter
{
    private static Matcher? _matcher;

    static IgnoreFilter()
    {
        InitializeMatcher();
    }

    private static void InitializeMatcher()
    {
        var config = ConfigurationManager.Instance.Config;
        _matcher = new Matcher();

        // Inclui tudo por padrão
        _matcher.AddInclude("**/*");

        // Exclui pastas ignoradas
        foreach (var folder in config.Filters.IgnoreFolders)
        {
            _matcher.AddExclude($"**/{folder}/**");
            _matcher.AddExclude(folder);
        }

        // Exclui arquivos ignorados
        foreach (var file in config.Filters.IgnoreFiles)
        {
            _matcher.AddExclude($"**/{file}");
            _matcher.AddExclude(file);
        }

        // Exclui extensões ignoradas
        foreach (var ext in config.Filters.IgnoreExtensions)
        {
            _matcher.AddExclude($"**/*{ext}");
        }

        // Exclui padrões customizados
        foreach (var pattern in config.Filters.CustomIgnorePatterns)
        {
            _matcher.AddExclude(pattern);
        }
    }

    public static bool MustIgnore(string name)
    {
        var config = ConfigurationManager.Instance.Config;

        // Verifica arquivos ocultos se configurado para ignorar
        if (!config.General.IncludeHiddenFiles && name.StartsWith(".") &&
            !name.EndsWith(".gitignore") && !name.EndsWith(".config"))
            return true;

        // Usa o matcher para verificar se deve ignorar
        var result = _matcher?.Match(name);
        return result == null || !result.HasMatches;
    }

    public static bool MustIgnore(string path, string rootPath)
    {
        try
        {
            var relativePath = Path.GetRelativePath(rootPath, path);
            var result = _matcher?.Match(relativePath);
            return result == null || !result.HasMatches;
        }
        catch
        {
            // Em caso de erro, usa o método simples
            return MustIgnore(Path.GetFileName(path));
        }
    }

    /// <summary>
    /// Recarrega as configurações de filtro
    /// </summary>
    public static void ReloadConfiguration()
    {
        InitializeMatcher();
    }

    /// <summary>
    /// Método de compatibilidade com o código existente
    /// </summary>
    public static bool MustIgnore(string name, bool isLegacyCall = false)
    {
        if (isLegacyCall)
        {
            // Comportamento original para compatibilidade
            var config = ConfigurationManager.Instance.Config;

            if (!config.General.IncludeHiddenFiles && name.StartsWith(".") &&
                !name.EndsWith(".gitignore") && !name.EndsWith(".config"))
                return true;

            return config.Filters.IgnoreFolders.Any(item =>
                string.Equals(item, name, StringComparison.OrdinalIgnoreCase));
        }

        return MustIgnore(name);
    }
}