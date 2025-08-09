namespace DesignProjectStructure.Helpers;

public static class ConsoleRenderer
{
    public static void DesignInterface()
    {
        Console.Clear();
        int width = Console.WindowWidth - 2;
        int height = Console.WindowHeight - 2;

        // Outer edge
        DrawBorder(0, 0, width + 1, height + 1, ConsoleColor.Cyan);

        // Header window (6 lines to accommodate more information)
        DrawBorder(1, 1, width - 1, 6, ConsoleColor.Yellow);

        // Frame window (middle of screen) - position 1.9
        int structureStart = 9;
        int structureHeight = height - 20;
        DrawBorder(1, structureStart, width - 1, structureHeight, ConsoleColor.Green);

        // Status window
        int statusStart = structureStart + structureHeight + 2;
        int statusHeight = height - statusStart;
        DrawBorder(1, statusStart, width - 1, statusHeight, ConsoleColor.Magenta);

        // Títulos das janelas
        Console.SetCursorPosition(3, 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("╣ PROJECT INFORMATIONS ╠");

        Console.SetCursorPosition(3, structureStart);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("╣ FILE STRUCTURE ╠");

        Console.SetCursorPosition(3, statusStart);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("╣ PROCESSING DATA ╠");

        Console.ResetColor();
    }

    public static void RenderHeader(string path, string file)
    {
        // Detect the project type
        var projectInfo = ProjectTypeDetector.DetectProjectType(path);

        // Line 1: Project Name + Type + Icon
        Console.SetCursorPosition(3, 2);
        string projectLine = $"{projectInfo.Icon} {Path.GetFileName(path)} - {projectInfo.Type}";
        if (projectLine.Length > Console.WindowWidth - 8)
            projectLine = projectLine.Substring(0, Console.WindowWidth - 11) + "...";
        Console.Write(projectLine.PadRight(Console.WindowWidth - 8));

        // Line 2: Language and Framework
        Console.SetCursorPosition(3, 3);
        string techLine = $"Tech: {projectInfo.Language}";
        if (!string.IsNullOrEmpty(projectInfo.Framework) && projectInfo.Framework != projectInfo.Language)
            techLine += $" | Framework: {projectInfo.Framework}";
        if (techLine.Length > Console.WindowWidth - 8)
            techLine = techLine.Substring(0, Console.WindowWidth - 11) + "...";
        Console.Write(techLine.PadRight(Console.WindowWidth - 8));

        // Line 3: Description
        Console.SetCursorPosition(3, 4);
        string descLine = $"Desc: {projectInfo.Description}";
        if (descLine.Length > Console.WindowWidth - 8)
            descLine = descLine.Substring(0, Console.WindowWidth - 11) + "...";
        Console.Write(descLine.PadRight(Console.WindowWidth - 8));

        // Line 4: Path
        Console.SetCursorPosition(3, 5);
        string caminhoTruncado = path.Length > Console.WindowWidth - 15
            ? "..." + path.Substring(path.Length - (Console.WindowWidth - 18))
            : path;
        Console.Write($"Path: {caminhoTruncado}".PadRight(Console.WindowWidth - 8));

        // Line 5: Output file + Date
        Console.SetCursorPosition(3, 6);
        Console.Write($"Output: {Path.GetFileName(file)} | {DateTime.Now:dd/MM/yyyy HH:mm:ss}".PadRight(Console.WindowWidth - 8));
    }

    public static void UpdateProgress(int count) { }

    public static void RenderFooter() { }

    static void DrawBorder(int x, int y, int width, int height, ConsoleColor color)
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

    public static void UpdateProgressBar(int percentual)
    {
        // Calcula a posição dinamicamente baseada no layout (ajustado para novo layout)
        int inicioStatus = Console.WindowHeight - 12;
        int posY = inicioStatus + 6;
        int larguraBarra = Math.Max(10, Console.WindowWidth - 25);
        int preenchido = (larguraBarra * percentual) / 100;

        Console.SetCursorPosition(3, posY);
        string textoProgresso = $"Progress: [{percentual:000}%] ";
        Console.Write(textoProgresso);

        // Verifica se há espaço suficiente para a barra
        int espacoRestante = Console.WindowWidth - 8 - textoProgresso.Length;
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
        int statusStart = Console.WindowHeight - 12;
        Console.SetCursorPosition(3, statusStart + 8);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"COMPLETED! File saved: {Path.GetFileName(outputFile)}");
        Console.ResetColor();

        Thread.Sleep(2000);
    }
}