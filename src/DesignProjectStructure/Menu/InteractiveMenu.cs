using DesignProjectStructure.Configuration;
using DesignProjectStructure.Models;

namespace DesignProjectStructure.Menu;


/// <summary>
/// Sistema de menu interativo com navegação por setas
/// </summary>
public class InteractiveMenu
{
    private readonly List<MenuOption> _options;
    private int _selectedIndex;
    private bool _isRunning;

    public InteractiveMenu()
    {
        _options = new List<MenuOption>();
        _selectedIndex = 0;
        _isRunning = true;
    }

    /// <summary>
    /// Adiciona uma opção ao menu
    /// </summary>
    public void AddOption(MenuOption option)
    {
        _options.Add(option);
    }

    /// <summary>
    /// Adiciona múltiplas opções ao menu
    /// </summary>
    public void AddOptions(params MenuOption[] options)
    {
        _options.AddRange(options);
    }

    /// <summary>
    /// Inicia o menu interativo
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        _isRunning = true;

        while (_isRunning)
        {
            DrawMenu();
            HandleInput();
        }

        Console.CursorVisible = true;
    }

    /// <summary>
    /// Para o menu
    /// </summary>
    public void Stop()
    {
        _isRunning = false;
    }

    /// <summary>
    /// Desenha o menu completo
    /// </summary>
    private void DrawMenu()
    {
        Console.Clear();
        DrawInterface();
        DrawHeader();
        DrawOptions();
        DrawFooter();
    }

    /// <summary>
    /// Desenha a interface base (bordas e janelas)
    /// </summary>
    private void DrawInterface()
    {
        int width = Console.WindowWidth - 2;
        int height = Console.WindowHeight - 2;

        // Borda externa
        DrawBorder(0, 0, width + 1, height + 1, ConsoleColor.Cyan);

        // Janela do cabeçalho
        DrawBorder(1, 1, width - 1, 6, ConsoleColor.Yellow);

        // Janela do menu (meio da tela)
        int menuStart = 9;
        int menuHeight = height - 16;
        DrawBorder(1, menuStart, width - 1, menuHeight, ConsoleColor.Green);

        // Janela de instruções
        int instructionStart = menuStart + menuHeight + 2;
        int instructionHeight = height - instructionStart;
        DrawBorder(1, instructionStart, width - 1, instructionHeight, ConsoleColor.Magenta);

        // Títulos das janelas
        Console.SetCursorPosition(3, 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("╣ DESIGN PROJECT STRUCTURE ╠");

        Console.SetCursorPosition(3, menuStart);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("╣ MAIN MENU ╠");

        Console.SetCursorPosition(3, instructionStart);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("╣ INSTRUCTIONS ╠");

        Console.ResetColor();
    }

    /// <summary>
    /// Desenha o cabeçalho com informações da aplicação
    /// </summary>
    private void DrawHeader()
    {
        var config = ConfigurationManager.Instance.Config;

        // Linha 1: Nome da aplicação
        Console.SetCursorPosition(3, 2);
        Console.Write($"🚀 Design Project Structure v1.0".PadRight(Console.WindowWidth - 8));

        // Linha 2: Descrição
        Console.SetCursorPosition(3, 3);
        Console.Write($"📁 Generate beautiful project structure documentation".PadRight(Console.WindowWidth - 8));

        // Linha 3: Configurações
        Console.SetCursorPosition(3, 4);
        Console.Write($"⚙️ Animation: {(config.General.ShowConsoleAnimation ? "ON" : "OFF")} | Formats: {string.Join(", ", config.Output.Formats)}".PadRight(Console.WindowWidth - 8));

        // Linha 4: Data/hora atual
        Console.SetCursorPosition(3, 5);
        Console.Write($"🕒 {DateTime.Now:dddd, dd/MM/yyyy HH:mm:ss}".PadRight(Console.WindowWidth - 8));

        // Linha 5: Diretório atual
        Console.SetCursorPosition(3, 6);
        var currentDir = Directory.GetCurrentDirectory();
        if (currentDir.Length > Console.WindowWidth - 15)
            currentDir = "..." + currentDir.Substring(currentDir.Length - (Console.WindowWidth - 18));
        Console.Write($"📍 Current: {currentDir}".PadRight(Console.WindowWidth - 8));
    }

    /// <summary>
    /// Desenha as opções do menu
    /// </summary>
    private void DrawOptions()
    {
        int startY = 11; // Começa após o título da janela do menu
        int maxVisibleOptions = Console.WindowHeight - 20; // Espaço disponível

        for (int i = 0; i < _options.Count && i < maxVisibleOptions; i++)
        {
            var option = _options[i];
            bool isSelected = i == _selectedIndex;

            DrawOption(3, startY + i, option, isSelected, i + 1);
        }

        // Se há mais opções do que cabem na tela
        if (_options.Count > maxVisibleOptions)
        {
            Console.SetCursorPosition(Console.WindowWidth - 15, startY + maxVisibleOptions - 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"... +{_options.Count - maxVisibleOptions}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Desenha uma opção individual do menu
    /// </summary>
    private void DrawOption(int x, int y, MenuOption option, bool isSelected, int number)
    {
        Console.SetCursorPosition(x, y);

        if (isSelected)
        {
            // Opção selecionada
            Console.BackgroundColor = option.IsSpecial ? ConsoleColor.DarkRed : ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            // Opção normal
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = option.IsEnabled ?
                (option.IsSpecial ? ConsoleColor.Red : ConsoleColor.White) :
                ConsoleColor.DarkGray;
        }

        // Formato: [N] 🔹 Title - Description
        string optionText = $"[{number}] {option.Icon} {option.Title}";

        // Adiciona descrição se couber
        int maxWidth = Console.WindowWidth - 12;
        if (optionText.Length < maxWidth - 3 && !string.IsNullOrEmpty(option.Description))
        {
            string separator = " - ";
            int remainingSpace = maxWidth - optionText.Length - separator.Length;
            if (remainingSpace > 10)
            {
                string description = option.Description;
                if (description.Length > remainingSpace)
                    description = description.Substring(0, remainingSpace - 3) + "...";
                optionText += separator + description;
            }
        }

        // Preenche o resto da linha para o efeito de seleção
        optionText = optionText.PadRight(maxWidth);

        Console.Write(optionText);
        Console.ResetColor();

        // Indicador de disponibilidade
        if (!option.IsEnabled)
        {
            Console.SetCursorPosition(Console.WindowWidth - 15, y);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("[DISABLED]");
            Console.ResetColor();
        }
        else if (option.ShortcutKey.HasValue)
        {
            Console.SetCursorPosition(Console.WindowWidth - 10, y);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"[{option.ShortcutKey}]");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Desenha as instruções na parte inferior
    /// </summary>
    private void DrawFooter()
    {
        int footerStart = Console.WindowHeight - 10;

        Console.SetCursorPosition(3, footerStart + 2);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("🎮 NAVIGATION:");
        Console.ResetColor();

        Console.SetCursorPosition(3, footerStart + 3);
        Console.Write("   ↑↓ Move selection  │  ENTER Execute  │  ESC Exit");

        Console.SetCursorPosition(3, footerStart + 4);
        Console.Write("   1-9 Quick select  │  H Help  │  C Configuration");

        Console.SetCursorPosition(3, footerStart + 6);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("💡 TIP: Use arrow keys to navigate and ENTER to select an option");
        Console.ResetColor();

        Console.SetCursorPosition(3, footerStart + 7);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"✅ Ready to process {_options.Count(o => o.IsEnabled && !o.IsSpecial)} available function(s)");
        Console.ResetColor();
    }

    /// <summary>
    /// Trata a entrada do usuário
    /// </summary>
    private void HandleInput()
    {
        var key = Console.ReadKey(true);

        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                MoveUp();
                break;

            case ConsoleKey.DownArrow:
                MoveDow();
                break;

            case ConsoleKey.Enter:
                ExecuteSelected();
                break;

            case ConsoleKey.Escape:
                _isRunning = false;
                break;

            case ConsoleKey.H:
                ShowHelp();
                break;

            case ConsoleKey.C:
                ShowConfiguration();
                break;

            // Quick select (1-9)
            case ConsoleKey.D1:
            case ConsoleKey.NumPad1:
                QuickSelect(0);
                break;
            case ConsoleKey.D2:
            case ConsoleKey.NumPad2:
                QuickSelect(1);
                break;
            case ConsoleKey.D3:
            case ConsoleKey.NumPad3:
                QuickSelect(2);
                break;
            case ConsoleKey.D4:
            case ConsoleKey.NumPad4:
                QuickSelect(3);
                break;
            case ConsoleKey.D5:
            case ConsoleKey.NumPad5:
                QuickSelect(4);
                break;
            case ConsoleKey.D6:
            case ConsoleKey.NumPad6:
                QuickSelect(5);
                break;
            case ConsoleKey.D7:
            case ConsoleKey.NumPad7:
                QuickSelect(6);
                break;
            case ConsoleKey.D8:
            case ConsoleKey.NumPad8:
                QuickSelect(7);
                break;
            case ConsoleKey.D9:
            case ConsoleKey.NumPad9:
                QuickSelect(8);
                break;
        }
    }

    #region Métodos de Navegação

    private void MoveDow()
    {
        if (_selectedIndex < _options.Count - 1)
            _selectedIndex++;
        else
            _selectedIndex = 0; // Volta para o início
    }

    private void MoveUp()
    {
        if (_selectedIndex > 0)
            _selectedIndex--;
        else
            _selectedIndex = _options.Count - 1; // Vai para o final
    }

    private void QuickSelect(int index)
    {
        if (index < _options.Count)
        {
            _selectedIndex = index;
            ExecuteSelected();
        }
    }

    private void ExecuteSelected()
    {
        if (_selectedIndex < _options.Count)
        {
            var option = _options[_selectedIndex];
            if (option.IsEnabled)
            {
                option.Action?.Invoke();
            }
            else
            {
                ShowMessage("⚠️ This option is currently disabled", ConsoleColor.Yellow);
            }
        }
    }

    #endregion

    #region Métodos Auxiliares

    private void ShowHelp()
    {
        ShowMessage("📖 Design Project Structure Help\n\n" +
                   "This tool generates beautiful documentation of your project structure.\n" +
                   "It supports multiple output formats (Markdown, HTML, JSON) and\n" +
                   "can be configured through the appsettings.json file.\n\n" +
                   "Press any key to continue...", ConsoleColor.Cyan);
    }

    private void ShowConfiguration()
    {
        var config = ConfigurationManager.Instance.Config;
        ShowMessage($"⚙️ Current Configuration\n\n" +
                   $"Animation: {(config.General.ShowConsoleAnimation ? "Enabled" : "Disabled")}\n" +
                   $"Output Formats: {string.Join(", ", config.Output.Formats)}\n" +
                   $"Default Output: {config.General.DefaultOutputPath}\n" +
                   $"Include Hidden Files: {config.General.IncludeHiddenFiles}\n\n" +
                   "Edit appsettings.json to modify these settings.\n\n" +
                   "Press any key to continue...", ConsoleColor.Green);
    }

    private void ShowMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.Clear();
        Console.ForegroundColor = color;

        // Centraliza a mensagem
        var lines = message.Split('\n');
        int startY = (Console.WindowHeight - lines.Length) / 2;

        foreach (var line in lines)
        {
            int startX = Math.Max(0, (Console.WindowWidth - line.Length) / 2);
            Console.SetCursorPosition(startX, startY++);
            Console.WriteLine(line);
        }

        Console.ResetColor();
        Console.ReadKey(true);
    }

    private void DrawBorder(int x, int y, int width, int height, ConsoleColor color)
    {
        Console.ForegroundColor = color;

        // Cantos
        Console.SetCursorPosition(x, y);
        Console.Write("╔");
        Console.SetCursorPosition(x + width, y);
        Console.Write("╗");
        Console.SetCursorPosition(x, y + height);
        Console.Write("╚");
        Console.SetCursorPosition(x + width, y + height);
        Console.Write("╝");

        // Linhas horizontais
        for (int i = x + 1; i < x + width; i++)
        {
            Console.SetCursorPosition(i, y);
            Console.Write("═");
            Console.SetCursorPosition(i, y + height);
            Console.Write("═");
        }

        // Linhas verticais
        for (int i = y + 1; i < y + height; i++)
        {
            Console.SetCursorPosition(x, i);
            Console.Write("║");
            Console.SetCursorPosition(x + width, i);
            Console.Write("║");
        }

        Console.ResetColor();
    }

    #endregion
}
