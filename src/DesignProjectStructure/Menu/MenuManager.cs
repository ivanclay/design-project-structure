using DesignProjectStructure.Menu;
using DesignProjectStructure.Models;

namespace DesignProjectStructure.Helpers;

/// <summary>
/// Gerenciador do menu principal da aplicação
/// </summary>
public class MenuManager
{
    private InteractiveMenu _menu;
    private bool _shouldExit;

    public MenuManager()
    {
        _menu = new InteractiveMenu();
        _shouldExit = false;
        SetupMenuOptions();
    }

    /// <summary>
    /// Configura todas as opções do menu
    /// </summary>
    private void SetupMenuOptions()
    {
        // Funcionalidades principais
        _menu.AddOption(new MenuOption(
            id: "generate-structure",
            title: "Generate Project Structure",
            description: "Analyze and generate project documentation",
            icon: "📁",
            isEnabled: true,
            action: ExecuteStructureGeneration
        ));

        _menu.AddOption(new MenuOption(
            id: "generate-consolidated",
            title: "Generate Consolidated Code File",
            description: "Create single file with all code for AI analysis",
            icon: "📑",
            isEnabled: true,
            action: ExecuteConsolidatedGeneration
        ));

        // Funcionalidades futuras (desabilitadas por enquanto)
        _menu.AddOption(new MenuOption(
            id: "analyze-dependencies",
            title: "Analyze Dependencies",
            description: "Analyze project dependencies and relationships",
            icon: "🔗",
            isEnabled: false, // Será habilitado em versões futuras
            action: () => ShowComingSoon("Dependency Analysis")
        ));

        _menu.AddOption(new MenuOption(
            id: "code-metrics",
            title: "Code Metrics",
            description: "Generate code quality and complexity metrics",
            icon: "📊",
            isEnabled: false, // Será habilitado em versões futuras
            action: () => ShowComingSoon("Code Metrics")
        ));

        _menu.AddOption(new MenuOption(
            id: "compare-projects",
            title: "Compare Projects",
            description: "Compare structures between different projects",
            icon: "⚖️",
            isEnabled: false, // Será habilitado em versões futuras
            action: () => ShowComingSoon("Project Comparison")
        ));

        // Separador visual
        _menu.AddOption(new MenuOption(
            id: "separator",
            title: "────────────────────────────────────",
            description: "",
            icon: "",
            isEnabled: false
        ));

        // Opções especiais
        _menu.AddOption(new MenuOption(
            id: "settings",
            title: "Settings",
            description: "Configure application settings",
            icon: "⚙️",
            isEnabled: true,
            action: ShowSettings
        ));

        _menu.AddOption(new MenuOption(
            id: "help",
            title: "Help",
            description: "Show help and documentation",
            icon: "❓",
            isEnabled: true,
            action: ShowHelp
        ));

        _menu.AddOption(new MenuOption(
            id: "about",
            title: "About",
            description: "About Design Project Structure",
            icon: "ℹ️",
            isEnabled: true,
            action: ShowAbout
        ));

        _menu.AddOption(new MenuOption(
            id: "exit",
            title: "Exit",
            description: "Exit the application",
            icon: "🚪",
            isEnabled: true,
            action: ExitApplication
        ));
    }

    /// <summary>
    /// Inicia o menu principal
    /// </summary>
    public void Start()
    {
        while (!_shouldExit)
        {
            try
            {
                _menu.Run();
            }
            catch (Exception ex)
            {
                ShowError($"Menu error: {ex.Message}");
            }
        }
    }

    #region Ações do Menu

    /// <summary>
    /// Executa a geração de estrutura do projeto (funcionalidade principal atual)
    /// </summary>
    private void ExecuteStructureGeneration()
    {
        try
        {
            _menu.Stop();

            // Chama a função de geração de estrutura do Program.cs
            Program.ExecuteStructureGeneration();

            // Após a execução, retorna ao menu se não deve sair
            if (_menu.IsRunning && !_shouldExit)
            {
                // Reinicia o menu
                _menu = new InteractiveMenu();
                SetupMenuOptions();
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error executing structure generation: {ex.Message}");

            // Reinicia o menu em caso de erro
            if (!_shouldExit)
            {
                _menu = new InteractiveMenu();
                SetupMenuOptions();
            }
        }
    }

    /// <summary>
    /// Executa a geração do arquivo consolidado
    /// </summary>
    private void ExecuteConsolidatedGeneration()
    {
        try
        {
            _menu.Stop();

            // Salva a configuração atual
            var config = Configuration.ConfigurationManager.Instance.Config;
            var originalFormats = new List<string>(config.Output.Formats);

            // Define temporariamente apenas o formato consolidado
            config.Output.Formats.Clear();
            config.Output.Formats.Add("consolidated");

            // Define um nome específico para o arquivo consolidado
            var originalOutputPath = config.General.DefaultOutputPath;
            config.General.DefaultOutputPath = "project-consolidated-for-ai.md";

            try
            {
                // Chama a função de geração
                Program.ExecuteStructureGeneration();
            }
            finally
            {
                // Restaura a configuração original
                config.Output.Formats.Clear();
                config.Output.Formats.AddRange(originalFormats);
                config.General.DefaultOutputPath = originalOutputPath;
            }

            // Após a execução, retorna ao menu se não deve sair
            if (_menu.IsRunning && !_shouldExit)
            {
                // Reinicia o menu
                _menu = new InteractiveMenu();
                SetupMenuOptions();
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error executing consolidated generation: {ex.Message}");

            // Reinicia o menu em caso de erro
            if (!_shouldExit)
            {
                _menu = new InteractiveMenu();
                SetupMenuOptions();
            }
        }
    }

    /// <summary>
    /// Placeholder para funcionalidades futuras
    /// </summary>
    private void ShowComingSoon(string featureName)
    {
        ShowMessage($"🚧 {featureName} - Coming Soon!\n\n" +
                   "This feature is planned for future versions.\n" +
                   "Stay tuned for updates!\n\n" +
                   "Press any key to return to menu...", ConsoleColor.Yellow);
    }

    /// <summary>
    /// Mostra as configurações da aplicação
    /// </summary>
    private void ShowSettings()
    {
        var config = Configuration.ConfigurationManager.Instance.Config;

        ShowMessage($"⚙️ Application Settings\n\n" +
                   $"📁 Default Output Path: {config.General.DefaultOutputPath}\n" +
                   $"🎬 Animation: {(config.General.ShowConsoleAnimation ? "Enabled" : "Disabled")}\n" +
                   $"⏱️  Animation Delay: {config.General.AnimationDelay}ms\n" +
                   $"📄 Output Formats: {string.Join(", ", config.Output.Formats)}\n" +
                   $"👁️  Include Hidden Files: {config.General.IncludeHiddenFiles}\n" +
                   $"📊 Include Statistics: {config.Output.IncludeStats}\n" +
                   $"🕒 Include Timestamp: {config.Output.IncludeTimestamp}\n\n" +
                   "To modify settings, edit 'appsettings.json' file.\n\n" +
                   "NEW: 📑 Consolidated format creates a single file\n" +
                   "with all code for easy AI assistant upload!\n\n" +
                   "Press any key to return to menu...", ConsoleColor.Cyan);
    }

    /// <summary>
    /// Mostra a ajuda da aplicação
    /// </summary>
    private void ShowHelp()
    {
        ShowMessage("📖 Design Project Structure - Help\n\n" +
                   "MAIN FEATURES:\n" +
                   "• Generate visual project structure documentation\n" +
                   "• Support for multiple output formats (MD, HTML, JSON)\n" +
                   "• Configurable filters and ignore patterns\n" +
                   "• Project type detection and analysis\n" +
                   "• Console animation and visual feedback\n\n" +
                   "NAVIGATION:\n" +
                   "• Use ↑↓ arrow keys to navigate menu\n" +
                   "• Press ENTER to select an option\n" +
                   "• Press 1-9 for quick selection\n" +
                   "• Press ESC to exit\n\n" +
                   "CONFIGURATION:\n" +
                   "• Edit 'appsettings.json' to customize behavior\n" +
                   "• Configure output formats, filters, and paths\n\n" +
                   "Press any key to return to menu...", ConsoleColor.Cyan);
    }

    /// <summary>
    /// Mostra informações sobre a aplicação
    /// </summary>
    private void ShowAbout()
    {
        ShowMessage("ℹ️ About Design Project Structure\n\n" +
                   "📋 Version: 1.0.0\n" +
                   "👨‍💻 A tool for generating beautiful project documentation\n\n" +
                   "FEATURES:\n" +
                   "• Multi-format output (Markdown, HTML, JSON)\n" +
                   "• Interactive console interface\n" +
                   "• Configurable filtering system\n" +
                   "• Project type auto-detection\n" +
                   "• Performance optimized processing\n\n" +
                   "SUPPORTED FORMATS:\n" +
                   "• Markdown (.md) - for documentation\n" +
                   "• HTML (.html) - for web viewing\n" +
                   "• JSON (.json) - for data analysis\n\n" +
                   "🚀 Built with performance and extensibility in mind\n\n" +
                   "Press any key to return to menu...", ConsoleColor.Green);
    }

    /// <summary>
    /// Sair da aplicação
    /// </summary>
    private void ExitApplication()
    {
        ShowMessage("👋 Thank you for using Design Project Structure!\n\n" +
                   "Goodbye!\n\n" +
                   "Press any key to exit...", ConsoleColor.Yellow);
        _shouldExit = true;
        _menu.Stop();
    }

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Mostra uma mensagem genérica
    /// </summary>
    private void ShowMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.Clear();
        Console.ForegroundColor = color;

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
        Console.ReadKey(true);
    }

    /// <summary>
    /// Mostra uma mensagem de erro
    /// </summary>
    private void ShowError(string message)
    {
        ShowMessage($"❌ ERROR\n\n{message}\n\nPress any key to continue...", ConsoleColor.Red);
    }

    #endregion
}