using DesignProjectStructure.Configuration;
using DesignProjectStructure.Models;
using System.Text;

namespace DesignProjectStructure.Helpers;

public class StructureGenerator
{
    // Constantes para layout consistente com ConsoleRenderer
    private const int CONTENT_OFFSET = 3;
    private const int HEADER_HEIGHT = 6;
    private const int STATUS_HEIGHT = 8;

    /// <summary>
    /// Calcula o layout das janelas baseado nas dimensões do console
    /// </summary>
    private static (int headerStart, int headerHeight, int structureStart, int structureHeight, int statusStart, int statusHeight) CalculateLayout()
    {
        int width = Math.Max(60, Console.WindowWidth - 2);
        int height = Math.Max(20, Console.WindowHeight - 2);

        // Altura mínima para cada seção
        int minHeaderHeight = 6;
        int minStatusHeight = 8;
        int minStructureHeight = 8;

        // Calcula alturas proporcionais
        int headerHeight = Math.Max(minHeaderHeight, height / 6); // ~16% da tela
        int statusHeight = Math.Max(minStatusHeight, height / 4);  // ~25% da tela

        // Posições
        int headerStart = 1;
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

    public static void UpdateStructure(string line, List<string> visualStructure)
    {
        var config = ConfigurationManager.Instance.Config;
        visualStructure.Add(line);

        // Só atualiza interface se animação estiver habilitada
        if (!config.General.ShowConsoleAnimation)
            return;

        var layout = CalculateLayout();

        // Cálculo correto das posições e limites da janela de estrutura
        int inicioJanelaEstrutura = layout.structureStart + 1; // Primeira linha após o título da janela
        int fimJanelaEstrutura = layout.statusStart - 2; // Duas linhas antes da próxima janela (margem + borda)
        int alturaDisponivelEstrutura = Math.Max(1, fimJanelaEstrutura - inicioJanelaEstrutura + 1);

        int inicioExibicao = Math.Max(0, visualStructure.Count - alturaDisponivelEstrutura);
        int maxWidth = Math.Max(1, Console.WindowWidth - 8);

        // Limpa a área da janela antes de escrever
        for (int i = 0; i < alturaDisponivelEstrutura; i++)
        {
            int linhaY = inicioJanelaEstrutura + i;
            if (linhaY <= fimJanelaEstrutura && linhaY < Console.WindowHeight - 1)
            {
                SafeSetCursorAndWrite(CONTENT_OFFSET, linhaY, new string(' ', maxWidth));
            }
        }

        // Exibe apenas as linhas que cabem na janela
        for (int i = 0; i < alturaDisponivelEstrutura && (inicioExibicao + i) < visualStructure.Count; i++)
        {
            int linhaY = inicioJanelaEstrutura + i;
            if (linhaY <= fimJanelaEstrutura && linhaY < Console.WindowHeight - 1)
            {
                string linhaExibicao = visualStructure[inicioExibicao + i];
                linhaExibicao = TruncateText(linhaExibicao, maxWidth);
                SafeSetCursorAndWrite(CONTENT_OFFSET, linhaY, linhaExibicao.PadRight(maxWidth));
            }
        }

        // Animação só se estiver habilitada
        if (config.General.ShowConsoleAnimation)
        {
            Thread.Sleep(config.General.AnimationDelay); // Usa delay da configuração
        }
    }

    public static void UpdateStatus(
        int contadorPastas,
        int contadorArquivos,
        int itensProcessados,
        int totalItens)
    {
        var config = ConfigurationManager.Instance.Config;

        // Só atualiza interface se animação estiver habilitada
        if (!config.General.ShowConsoleAnimation)
            return;

        var layout = CalculateLayout();
        int maxWidth = Math.Max(1, Console.WindowWidth - 8);

        // Linhas de status dentro da janela de status
        var statusLines = new[]
        {
            $"[DIR] FOLDERS FOUND: {contadorPastas:000}",
            $"[FILE] FILES FOUND: {contadorArquivos:000}",
            $"[OK] PROCESSED: {itensProcessados:000} of {totalItens:000}",
            "" // Linha vazia antes da barra de progresso
        };

        for (int i = 0; i < statusLines.Length && i < layout.statusHeight - 3; i++)
        {
            int linhaY = layout.statusStart + 2 + i; // +2 para pular título e borda
            if (linhaY < Console.WindowHeight - 1)
            {
                SafeSetCursorAndWrite(CONTENT_OFFSET, linhaY, TruncateText(statusLines[i], maxWidth).PadRight(maxWidth));
            }
        }

        int progresso = totalItens > 0 ? (itensProcessados * 100) / totalItens : 0;
        ConsoleRenderer.UpdateProgressBar(progresso);
    }

    /// <summary>
    /// Atualiza o status final quando animação está desabilitada
    /// </summary>
    public static void UpdateFinalStatus(
        int contadorPastas,
        int contadorArquivos,
        int itensProcessados,
        int totalItens)
    {
        var layout = CalculateLayout();
        int maxWidth = Math.Max(1, Console.WindowWidth - 8);

        // Linhas de status dentro da janela de status
        var statusLines = new[]
        {
            $"[DIR] FOLDERS FOUND: {contadorPastas:000}",
            $"[FILE] FILES FOUND: {contadorArquivos:000}",
            $"[OK] PROCESSED: {itensProcessados:000} of {totalItens:000}",
            "" // Linha vazia antes da barra de progresso
        };

        for (int i = 0; i < statusLines.Length && i < layout.statusHeight - 3; i++)
        {
            int linhaY = layout.statusStart + 2 + i; // +2 para pular título e borda
            if (linhaY < Console.WindowHeight - 1)
            {
                SafeSetCursorAndWrite(CONTENT_OFFSET, linhaY, TruncateText(statusLines[i], maxWidth).PadRight(maxWidth));
            }
        }

        ConsoleRenderer.UpdateProgressBar(100);
    }

    public static int CounterItens(string caminho)
    {
        int totalItens = 0;
        try
        {
            if (Directory.Exists(caminho))
            {
                totalItens++;
                var itens = Directory.GetFileSystemEntries(caminho);
                foreach (var item in itens)
                {
                    if (!IgnoreFilter.MustIgnore(Path.GetFileName(item)))
                    {
                        totalItens += CounterItens(item);
                    }
                }
            }
            else
            {
                totalItens++;
            }
        }
        catch { }
        return totalItens;
    }

    /// <summary>
    /// Define posição do cursor e escreve texto de forma segura
    /// </summary>
    private static void SafeSetCursorAndWrite(int x, int y, string text)
    {
        try
        {
            if (x >= 0 && y >= 0 && x < Console.WindowWidth && y < Console.WindowHeight)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(text);
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
}