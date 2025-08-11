namespace DesignProjectStructure.Helpers;

public static class ConsoleRenderer
{
    // Constantes para layout mais consistente
    private const int BORDER_OFFSET = 1;
    private const int CONTENT_OFFSET = 3;
    private const int HEADER_HEIGHT = 6;
    private const int STATUS_HEIGHT = 8;

    public static void DesignInterface()
    {
        Console.Clear();

        // Calcula dimensões seguras
        int width = Math.Max(60, Console.WindowWidth - 2);
        int height = Math.Max(20, Console.WindowHeight - 2);

        // Calcula posições das janelas
        var layout = CalculateLayout(width, height);

        // Outer edge
        DrawBorder(0, 0, width + 1, height + 1, ConsoleColor.Cyan);

        // Header window
        DrawBorder(BORDER_OFFSET, layout.headerStart, width - 1, layout.headerHeight, ConsoleColor.Yellow);

        // Structure window (middle)
        DrawBorder(BORDER_OFFSET, layout.structureStart, width - 1, layout.structureHeight, ConsoleColor.Green);

        // Status window (bottom)
        DrawBorder(BORDER_OFFSET, layout.statusStart, width - 1, layout.statusHeight, ConsoleColor.Magenta);

        // Títulos das janelas com posicionamento seguro
        SafeSetCursorAndWrite(CONTENT_OFFSET, layout.headerStart, "╣ PROJECT INFORMATIONS ╠", ConsoleColor.Yellow);
        SafeSetCursorAndWrite(CONTENT_OFFSET, layout.structureStart, "╣ FILE STRUCTURE ╠", ConsoleColor.Green);
        SafeSetCursorAndWrite(CONTENT_OFFSET, layout.statusStart, "╣ PROCESSING DATA ╠", ConsoleColor.Magenta);

        Console.ResetColor();
    }

    /// <summary>
    /// Calcula o layout das janelas baseado nas dimensões disponíveis
    /// </summary>
    private static (int headerStart, int headerHeight, int structureStart, int structureHeight, int statusStart, int statusHeight) CalculateLayout(int width, int height)
    {
        // Altura mínima para cada seção
        int minHeaderHeight = 6;
        int minStatusHeight = 8;
        int minStructureHeight = 8;

        // Calcula alturas proporcionais
        int headerHeight = Math.Max(minHeaderHeight, height / 6); // ~16% da tela
        int statusHeight = Math.Max(minStatusHeight, height / 4);  // ~25% da tela

        // Posições
        int headerStart = BORDER_OFFSET;
        int statusStart = height - statusHeight;
        int structureStart = headerStart + headerHeight + 2; // +2 para espaçamento
        int structureHeight = statusStart - structureStart - 1; // -1 para espaçamento

        // Ajusta se a estrutura ficou muito pequena
        if (structureHeight < minStructureHeight)
        {
            // Reduz outras seções proporcionalmente
            int totalReduction = minStructureHeight - structureHeight;
            int headerReduction = Math.Min(headerHeight - minHeaderHeight, totalReduction / 2);
            int statusReduction = totalReduction - headerReduction;

            headerHeight = Math.Max(minHeaderHeight, headerHeight - headerReduction);
            statusHeight = Math.Max(minStatusHeight, statusHeight - statusReduction);

            // Recalcula posições
            structureStart = headerStart + headerHeight + 2;
            statusStart = height - statusHeight;
            structureHeight = statusStart - structureStart - 1;
        }

        return (headerStart, headerHeight, structureStart, Math.Max(minStructureHeight, structureHeight), statusStart, statusHeight);
    }

    public static void RenderHeader(string path, string file)
    {
        var layout = CalculateLayout(Console.WindowWidth - 2, Console.WindowHeight - 2);
        int maxWidth = Console.WindowWidth - 8;

        // Detect the project type
        var projectInfo = ProjectTypeDetector.DetectProjectType(path);

        var headerLines = new[]
        {
            $"{projectInfo.Icon} {Path.GetFileName(path)} - {projectInfo.Type}",
            $"Tech: {projectInfo.Language}" + (!string.IsNullOrEmpty(projectInfo.Framework) && projectInfo.Framework != projectInfo.Language ? $" | Framework: {projectInfo.Framework}" : ""),
            $"Desc: {projectInfo.Description}",
            TruncatePath($"Path: {path}", maxWidth),
            $"Output: {Path.GetFileName(file)} | {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
        };

        for (int i = 0; i < headerLines.Length && i < layout.headerHeight - 2; i++)
        {
            SafeSetCursorAndWrite(CONTENT_OFFSET, layout.headerStart + 2 + i, TruncateText(headerLines[i], maxWidth));
        }
    }

    public static void UpdateProgress(int count)
    {
        // Método mantido por compatibilidade
    }

    public static void RenderFooter()
    {
        // Método mantido por compatibilidade
    }

    static void DrawBorder(int x, int y, int width, int height, ConsoleColor color)
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

    public static void UpdateProgressBar(int percentual)
    {
        var layout = CalculateLayout(Console.WindowWidth - 2, Console.WindowHeight - 2);
        int posY = layout.statusStart + 6;

        // Verifica se a posição Y é válida
        if (posY >= Console.WindowHeight - 1)
        {
            posY = Console.WindowHeight - 3; // Posição de fallback
        }

        int larguraBarra = Math.Max(10, Console.WindowWidth - 25);
        int preenchido = (larguraBarra * percentual) / 100;

        SafeSetCursorAndWrite(CONTENT_OFFSET, posY, $"Progress: [{percentual:000}%] ");

        // Verifica se há espaço suficiente para a barra
        int espacoRestante = Console.WindowWidth - 8 - 20;
        if (espacoRestante > 0)
        {
            larguraBarra = Math.Min(larguraBarra, espacoRestante);
            preenchido = (larguraBarra * percentual) / 100;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string('█', preenchido));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', larguraBarra - preenchido));
            Console.ResetColor();
        }
    }

    public static void FinalMessage(string outputFile)
    {
        var layout = CalculateLayout(Console.WindowWidth - 2, Console.WindowHeight - 2);
        int messageY = Math.Min(layout.statusStart + 8, Console.WindowHeight - 3);

        SafeSetCursorAndWrite(CONTENT_OFFSET, messageY, "COMPLETED! File saved: " + Path.GetFileName(outputFile), ConsoleColor.Green);
        Thread.Sleep(2000);
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
}