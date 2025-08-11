using DesignProjectStructure.Configuration;
using DesignProjectStructure.Models;

namespace DesignProjectStructure.Menu;

/// <summary>
/// Sistema de menu interativo com navegação por setas e rolagem automática
/// </summary>
public class InteractiveMenu
{
    private readonly List<MenuOption> _options;
    private int _selectedIndex;
    private int _scrollOffset;
    private bool _isRunning;

    // Constantes para layout consistente
    private const int BORDER_OFFSET = 1;
    private const int CONTENT_OFFSET = 3;

    public InteractiveMenu()
    {
        _options = new List<MenuOption>();
        _selectedIndex = 0;
        _scrollOffset = 0;
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
            DrawMenuInterface();
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

    public bool IsRunning
        { get { return _isRunning; } }

    /// <summary>
    /// Calcula o layout das janelas baseado nas dimensões do console
    /// </summary>
    private (int headerStart, int headerHeight, int menuStart, int menuHeight, int footerStart, int footerHeight) CalculateLayout()
    {
        int width = Math.Max(60, Console.WindowWidth - 2);
        int height = Math.Max(20, Console.WindowHeight - 2);

        // Alturas mínimas e proporcionais
        int minHeaderHeight = 8;
        int minFooterHeight = 8;
        int minMenuHeight = 6;

        // Calcula alturas proporcionais
        int headerHeight = Math.Max(minHeaderHeight, height / 4);  // ~25% da tela
        int footerHeight = Math.Max(minFooterHeight, height / 4);  // ~25% da tela

        // Posições
        int headerStart = BORDER_OFFSET;
        int footerStart = height - footerHeight;
        int menuStart = headerStart + headerHeight + 2; // +2 para espaçamento
        int menuHeight = footerStart - menuStart - 1; // -1 para espaçamento

        // Ajusta se o menu ficou muito pequeno
        if (menuHeight < minMenuHeight)
        {
            int totalReduction = minMenuHeight - menuHeight;
            int headerReduction = Math.Min(headerHeight - minHeaderHeight, totalReduction / 2);
            int footerReduction = totalReduction - headerReduction;

            headerHeight = Math.Max(minHeaderHeight, headerHeight - headerReduction);
            footerHeight = Math.Max(minFooterHeight, footerHeight - footerReduction);

            // Recalcula posições
            menuStart = headerStart + headerHeight + 2;
            footerStart = height - footerHeight;
            menuHeight = footerStart - menuStart - 1;
        }

        return (headerStart, headerHeight, menuStart, Math.Max(minMenuHeight, menuHeight), footerStart, footerHeight);
    }

    /// <summary>
    /// Desenha a interface completa do menu (exclusiva)
    /// </summary>
    private void DrawMenuInterface()
    {
        Console.Clear();
        var layout = CalculateLayout();

        DrawMenuBorders(layout);
        DrawMenuHeader(layout);
        DrawMenuOptions(layout);
        DrawMenuInstructions(layout);
    }

    /// <summary>
    /// Desenha as bordas específicas do menu
    /// </summary>
    private void DrawMenuBorders((int headerStart, int headerHeight, int menuStart, int menuHeight, int footerStart, int footerHeight) layout)
    {
        int width = Console.WindowWidth - 2;
        int height = Console.WindowHeight - 2;

        // Borda externa principal
        DrawBorder(0, 0, width + 1, height + 1, ConsoleColor.Cyan);

        // Janela do cabeçalho do menu
        DrawBorder(BORDER_OFFSET, layout.headerStart, width - 1, layout.headerHeight, ConsoleColor.Yellow);

        // Janela principal do menu (centro da tela)
        DrawBorder(BORDER_OFFSET, layout.menuStart, width - 1, layout.menuHeight, ConsoleColor.Green);

        // Janela de instruções (parte inferior)
        DrawBorder(BORDER_OFFSET, layout.footerStart, width - 1, layout.footerHeight, ConsoleColor.Magenta);

        // Títulos das janelas
        Console.SetCursorPosition(CONTENT_OFFSET, layout.headerStart);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("╣ DESIGN PROJECT STRUCTURE ╠");

        Console.SetCursorPosition(CONTENT_OFFSET, layout.menuStart);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("╣ MAIN MENU ╠");

        Console.SetCursorPosition(CONTENT_OFFSET, layout.footerStart);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("╣ NAVIGATION HELP ╠");

        Console.ResetColor();
    }

    /// <summary>
    /// Desenha o cabeçalho específico do menu
    /// </summary>
    private void DrawMenuHeader((int headerStart, int headerHeight, int menuStart, int menuHeight, int footerStart, int footerHeight) layout)
    {
        var config = ConfigurationManager.Instance.Config;
        int maxWidth = Console.WindowWidth - 8;

        var headerLines = new[]
        {
            "🚀 Design Project Structure v1.0",
            "📁 Generate beautiful project structure documentation",
            $"⚙️ Animation: {(config.General.ShowConsoleAnimation ? "ON" : "OFF")} | Output: {string.Join(", ", config.Output.Formats)}",
            $"🕒 {DateTime.Now:dddd, dd/MM/yyyy HH:mm:ss}",
            $"📍 Current Directory: {TruncatePath(Directory.GetCurrentDirectory(), maxWidth - 25)}",
            "✅ Ready to analyze project structure",
            new string('═', Math.Min(maxWidth, 50))
        };

        for (int i = 0; i < headerLines.Length && i < layout.headerHeight - 2; i++)
        {
            SafeSetCursorAndWrite(CONTENT_OFFSET, layout.headerStart + 2 + i,
                TruncateText(headerLines[i], maxWidth));
        }
    }

    /// <summary>
    /// Desenha as opções do menu com sistema de rolagem
    /// </summary>
    private void DrawMenuOptions((int headerStart, int headerHeight, int menuStart, int menuHeight, int footerStart, int footerHeight) layout)
    {
        // Calcula área disponível para opções (descontando bordas e título)
        int startY = layout.menuStart + 2; // +2 para pular borda e título
        int endY = layout.menuStart + layout.menuHeight - 1; // -1 para não sobrepor a borda inferior
        int availableLines = endY - startY;

        if (availableLines <= 0) return;

        // Ajusta scroll offset baseado na seleção atual
        AdjustScrollOffset(availableLines);

        // Limpa a área do menu
        int maxWidth = Console.WindowWidth - 8;
        for (int i = 0; i < availableLines; i++)
        {
            int lineY = startY + i;
            SafeSetCursorAndWrite(CONTENT_OFFSET, lineY, new string(' ', maxWidth));
        }

        // Desenha as opções visíveis
        int visibleOptionsCount = 0;
        for (int i = _scrollOffset; i < _options.Count && visibleOptionsCount < availableLines; i++)
        {
            var option = _options[i];
            bool isSelected = i == _selectedIndex;
            int lineY = startY + visibleOptionsCount;

            if (option.Id == "separator")
            {
                DrawMenuSeparator(CONTENT_OFFSET, lineY, maxWidth);
            }
            else
            {
                // Calcula o número da opção (excluindo separadores)
                int optionNumber = CalculateOptionNumber(i);
                DrawMenuOption(CONTENT_OFFSET, lineY, option, isSelected, optionNumber, maxWidth);
            }

            visibleOptionsCount++;
        }

        // Indicadores de scroll
        DrawScrollIndicators(layout, availableLines);
    }

    /// <summary>
    /// Ajusta o offset de scroll baseado na opção selecionada
    /// </summary>
    private void AdjustScrollOffset(int availableLines)
    {
        // Se a opção selecionada está acima da área visível, ajusta scroll para cima
        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
        }
        // Se a opção selecionada está abaixo da área visível, ajusta scroll para baixo
        else if (_selectedIndex >= _scrollOffset + availableLines)
        {
            _scrollOffset = _selectedIndex - availableLines + 1;
        }

        // Garante que o scroll não seja negativo
        _scrollOffset = Math.Max(0, _scrollOffset);

        // Garante que o scroll não ultrapasse o número de opções
        _scrollOffset = Math.Min(_scrollOffset, Math.Max(0, _options.Count - availableLines));
    }

    /// <summary>
    /// Desenha indicadores de scroll (setas para mais opções)
    /// </summary>
    private void DrawScrollIndicators((int headerStart, int headerHeight, int menuStart, int menuHeight, int footerStart, int footerHeight) layout, int availableLines)
    {
        int rightX = Console.WindowWidth - 10;

        // Indicador de scroll para cima
        if (_scrollOffset > 0)
        {
            SafeSetCursorAndWrite(rightX, layout.menuStart + 1, "▲ More", ConsoleColor.DarkGray);
        }

        // Indicador de scroll para baixo
        if (_scrollOffset + availableLines < _options.Count)
        {
            SafeSetCursorAndWrite(rightX, layout.menuStart + layout.menuHeight - 2, "▼ More", ConsoleColor.DarkGray);
        }

        // Mostra posição atual
        if (_options.Count > availableLines)
        {
            int centerY = layout.menuStart + (layout.menuHeight / 2);
            string scrollInfo = $"{_selectedIndex + 1}/{_options.Count}";
            SafeSetCursorAndWrite(rightX, centerY, scrollInfo, ConsoleColor.DarkCyan);
        }
    }

    /// <summary>
    /// Calcula o número da opção (ignorando separadores)
    /// </summary>
    private int CalculateOptionNumber(int index)
    {
        int number = 1;
        for (int i = 0; i < index && i < _options.Count; i++)
        {
            if (_options[i].Id != "separator")
            {
                if (i == index) return number;
                number++;
            }
        }
        return number;
    }

    /// <summary>
    /// Desenha uma opção individual do menu
    /// </summary>
    private void DrawMenuOption(int x, int y, MenuOption option, bool isSelected, int number, int maxWidth)
    {
        Console.SetCursorPosition(x, y);

        // Cores baseadas no estado da opção
        if (isSelected)
        {
            Console.BackgroundColor = option.IsSpecial ? ConsoleColor.DarkRed : ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = option.IsEnabled ?
                (option.IsSpecial ? ConsoleColor.Red : ConsoleColor.White) :
                ConsoleColor.DarkGray;
        }

        // Formato: [N] 🔹 Title - Description
        string prefix = option.IsSpecial ? "[*]" : $"[{number}]";
        string optionText = $"{prefix} {option.Icon} {option.Title}";

        // Adiciona descrição se houver espaço
        int remainingSpace = maxWidth - optionText.Length - 3; // -3 para " - "
        if (remainingSpace > 10 && !string.IsNullOrEmpty(option.Description))
        {
            string description = option.Description;
            if (description.Length > remainingSpace)
                description = description.Substring(0, remainingSpace - 3) + "...";
            optionText += " - " + description;
        }

        // Preenche o resto da linha para destacar a seleção
        optionText = optionText.PadRight(maxWidth);
        Console.Write(optionText);

        // Indicadores de status
        if (!option.IsEnabled)
        {
            Console.SetCursorPosition(Console.WindowWidth - 15, y);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("[DISABLED]");
        }
        else if (option.ShortcutKey.HasValue)
        {
            Console.SetCursorPosition(Console.WindowWidth - 12, y);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write($"[{option.ShortcutKey}]");
        }

        Console.ResetColor();
    }

    /// <summary>
    /// Desenha um separador visual no menu
    /// </summary>
    private void DrawMenuSeparator(int x, int y, int maxWidth)
    {
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = ConsoleColor.DarkGray;

        string separator = new string('─', Math.Min(maxWidth, Console.WindowWidth - x - 5));
        Console.Write(separator);

        Console.ResetColor();
    }

    /// <summary>
    /// Desenha as instruções de navegação
    /// </summary>
    private void DrawMenuInstructions((int headerStart, int headerHeight, int menuStart, int menuHeight, int footerStart, int footerHeight) layout)
    {
        int maxWidth = Console.WindowWidth - 8;

        var instructionLines = new[]
        {
            "🎮 NAVIGATION CONTROLS:",
            "↑↓ Navigate  │  ENTER Select  │  ESC Exit",
            "1-9 Quick Select  │  H Help  │  C Config",
            "",
            "💡 TIP: Use arrow keys to navigate through options and ENTER to execute",
            $"✅ {_options.Count(o => o.IsEnabled && !o.IsSpecial)} function(s) available | Current: {_selectedIndex + 1}/{_options.Count}"
        };

        for (int i = 0; i < instructionLines.Length && i < layout.footerHeight - 2; i++)
        {
            int lineY = layout.footerStart + 2 + i;
            ConsoleColor color = i == 0 ? ConsoleColor.Cyan :
                                i == 4 ? ConsoleColor.Yellow :
                                i == 5 ? ConsoleColor.Green : ConsoleColor.White;

            SafeSetCursorAndWrite(CONTENT_OFFSET, lineY,
                TruncateText(instructionLines[i], maxWidth), color);
        }
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
                MoveDown();
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

            // Seleção rápida (1-9)
            case ConsoleKey.D1 or ConsoleKey.NumPad1:
                QuickSelect(0);
                break;
            case ConsoleKey.D2 or ConsoleKey.NumPad2:
                QuickSelect(1);
                break;
            case ConsoleKey.D3 or ConsoleKey.NumPad3:
                QuickSelect(2);
                break;
            case ConsoleKey.D4 or ConsoleKey.NumPad4:
                QuickSelect(3);
                break;
            case ConsoleKey.D5 or ConsoleKey.NumPad5:
                QuickSelect(4);
                break;
            case ConsoleKey.D6 or ConsoleKey.NumPad6:
                QuickSelect(5);
                break;
            case ConsoleKey.D7 or ConsoleKey.NumPad7:
                QuickSelect(6);
                break;
            case ConsoleKey.D8 or ConsoleKey.NumPad8:
                QuickSelect(7);
                break;
            case ConsoleKey.D9 or ConsoleKey.NumPad9:
                QuickSelect(8);
                break;

            // Page Up / Page Down para rolagem rápida
            case ConsoleKey.PageUp:
                PageUp();
                break;
            case ConsoleKey.PageDown:
                PageDown();
                break;

            // Home / End para ir ao início/fim
            case ConsoleKey.Home:
                GoToFirst();
                break;
            case ConsoleKey.End:
                GoToLast();
                break;
        }
    }

    #region Métodos de Navegação

    private void MoveDown()
    {
        int originalIndex = _selectedIndex;

        do
        {
            if (_selectedIndex < _options.Count - 1)
                _selectedIndex++;
            else
                _selectedIndex = 0; // Volta para o início
        } while (_options[_selectedIndex].Id == "separator" && _selectedIndex != originalIndex);
    }

    private void MoveUp()
    {
        int originalIndex = _selectedIndex;

        do
        {
            if (_selectedIndex > 0)
                _selectedIndex--;
            else
                _selectedIndex = _options.Count - 1; // Vai para o final
        } while (_options[_selectedIndex].Id == "separator" && _selectedIndex != originalIndex);
    }

    private void PageDown()
    {
        var layout = CalculateLayout();
        int availableLines = layout.menuHeight - 3; // Descontar bordas e título
        int targetIndex = Math.Min(_selectedIndex + availableLines, _options.Count - 1);

        // Move para a próxima opção válida
        _selectedIndex = targetIndex;
        while (_selectedIndex > 0 && _options[_selectedIndex].Id == "separator")
        {
            _selectedIndex--;
        }
    }

    private void PageUp()
    {
        var layout = CalculateLayout();
        int availableLines = layout.menuHeight - 3; // Descontar bordas e título
        int targetIndex = Math.Max(_selectedIndex - availableLines, 0);

        // Move para a próxima opção válida
        _selectedIndex = targetIndex;
        while (_selectedIndex < _options.Count - 1 && _options[_selectedIndex].Id == "separator")
        {
            _selectedIndex++;
        }
    }

    private void GoToFirst()
    {
        _selectedIndex = 0;
        while (_selectedIndex < _options.Count - 1 && _options[_selectedIndex].Id == "separator")
        {
            _selectedIndex++;
        }
    }

    private void GoToLast()
    {
        _selectedIndex = _options.Count - 1;
        while (_selectedIndex > 0 && _options[_selectedIndex].Id == "separator")
        {
            _selectedIndex--;
        }
    }

    private void QuickSelect(int index)
    {
        // Converte o índice numérico para o índice real (ignorando separadores)
        int realIndex = 0;
        int optionCount = 0;

        for (int i = 0; i < _options.Count; i++)
        {
            if (_options[i].Id != "separator")
            {
                if (optionCount == index)
                {
                    realIndex = i;
                    break;
                }
                optionCount++;
            }
        }

        if (realIndex < _options.Count && _options[realIndex].Id != "separator")
        {
            _selectedIndex = realIndex;
            ExecuteSelected();
        }
    }

    private void ExecuteSelected()
    {
        if (_selectedIndex < _options.Count)
        {
            var option = _options[_selectedIndex];
            if (option.IsEnabled && option.Id != "separator")
            {
                option.Action?.Invoke();
            }
            else if (option.Id == "separator")
            {
                // Não faz nada para separadores
                return;
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
                   "Features:\n" +
                   "• Multi-format output generation\n" +
                   "• Smart project type detection\n" +
                   "• Configurable filtering system\n" +
                   "• Interactive console interface\n" +
                   "• Performance optimized processing\n\n" +
                   "Navigation:\n" +
                   "• ↑↓ Arrow keys to navigate\n" +
                   "• Page Up/Down for fast scrolling\n" +
                   "• Home/End to go to first/last option\n" +
                   "• 1-9 for quick selection\n\n" +
                   "Press any key to return to menu...", ConsoleColor.Cyan);
    }

    private void ShowConfiguration()
    {
        var config = ConfigurationManager.Instance.Config;
        ShowMessage($"⚙️ Current Configuration\n\n" +
                   $"🎬 Animation: {(config.General.ShowConsoleAnimation ? "Enabled" : "Disabled")}\n" +
                   $"📄 Output Formats: {string.Join(", ", config.Output.Formats)}\n" +
                   $"📁 Default Output: {config.General.DefaultOutputPath}\n" +
                   $"👁️ Include Hidden Files: {config.General.IncludeHiddenFiles}\n" +
                   $"⏱️ Animation Delay: {config.General.AnimationDelay}ms\n" +
                   $"📊 Include Statistics: {config.Output.IncludeStats}\n" +
                   $"🕒 Include Timestamp: {config.Output.IncludeTimestamp}\n\n" +
                   "To modify these settings, edit the 'appsettings.json' file\n" +
                   "in the application directory.\n\n" +
                   "Press any key to return to menu...", ConsoleColor.Green);
    }

    private void ShowMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.Clear();
        Console.ForegroundColor = color;

        // Centraliza a mensagem na tela
        var lines = message.Split('\n');
        int startY = Math.Max(1, (Console.WindowHeight - lines.Length) / 2);

        foreach (var line in lines)
        {
            int startX = Math.Max(0, (Console.WindowWidth - line.Length) / 2);
            SafeSetCursorAndWrite(startX, startY++, line);
        }

        Console.ResetColor();
        Console.ReadKey(true);
    }

    private void DrawBorder(int x, int y, int width, int height, ConsoleColor color)
    {
        Console.ForegroundColor = color;

        try
        {
            // Verifica limites antes de desenhar
            if (y >= Console.WindowHeight || x >= Console.WindowWidth ||
                y + height >= Console.WindowHeight || x + width >= Console.WindowWidth)
            {
                Console.ResetColor();
                return;
            }

            // Cantos
            SafeSetCursorAndWrite(x, y, "╔");
            SafeSetCursorAndWrite(x + width, y, "╗");
            SafeSetCursorAndWrite(x, y + height, "╚");
            SafeSetCursorAndWrite(x + width, y + height, "╝");

            // Linhas horizontais
            for (int i = x + 1; i < x + width && i < Console.WindowWidth - 1; i++)
            {
                SafeSetCursorAndWrite(i, y, "═");
                SafeSetCursorAndWrite(i, y + height, "═");
            }

            // Linhas verticais
            for (int i = y + 1; i < y + height && i < Console.WindowHeight - 1; i++)
            {
                SafeSetCursorAndWrite(x, i, "║");
                SafeSetCursorAndWrite(x + width, i, "║");
            }
        }
        catch
        {
            // Ignora erros de desenho
        }

        Console.ResetColor();
    }

    /// <summary>
    /// Define posição do cursor e escreve texto de forma segura
    /// </summary>
    private static void SafeSetCursorAndWrite(int x, int y, string text, ConsoleColor? color = null)
    {
        try
        {
            if (x >= 0 && y >= 0 && x < Console.WindowWidth && y < Console.WindowHeight)
            {
                Console.SetCursorPosition(x, y);
                if (color.HasValue)
                {
                    var originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = color.Value;
                    Console.Write(text);
                    Console.ForegroundColor = originalColor;
                }
                else
                {
                    Console.Write(text);
                }
            }
        }
        catch
        {
            // Ignora erros de posicionamento do cursor
        }
    }

    /// <summary>
    /// Trunca texto para caber na largura especificada
    /// </summary>
    private static string TruncateText(string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text) || maxWidth <= 0) return string.Empty;

        if (text.Length <= maxWidth) return text;

        return maxWidth > 3 ? text.Substring(0, maxWidth - 3) + "..." : text.Substring(0, maxWidth);
    }

    /// <summary>
    /// Trunca caminho para exibição
    /// </summary>
    private static string TruncatePath(string path, int maxWidth)
    {
        if (string.IsNullOrEmpty(path) || maxWidth <= 0) return string.Empty;

        if (path.Length <= maxWidth) return path;

        return maxWidth > 3 ? "..." + path.Substring(path.Length - maxWidth + 3) : path.Substring(0, maxWidth);
    }

    #endregion
}