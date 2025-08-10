using DesignProjectStructure.Configuration;
using DesignProjectStructure.Models;
using System.Text;

namespace DesignProjectStructure.Helpers;

public class StructureGenerator
{
    public static void UpdateStructure(string line, List<string> visualStructure)
    {
        var config = ConfigurationManager.Instance.Config;
        visualStructure.Add(line);

        // Só atualiza interface se animação estiver habilitada
        if (!config.General.ShowConsoleAnimation)
            return;

        // Cálculo correto das posições e limites da janela (ajustado para o novo cabeçalho)
        int inicioJanelaEstrutura = 10; // Primeira linha após o título da janela (linha 9 + 1)
        int fimJanelaEstrutura = Console.WindowHeight - 12 - 1; // Uma linha antes da próxima janela
        int alturaDisponivelEstrutura = fimJanelaEstrutura - inicioJanelaEstrutura; // Altura real disponível

        // Garante que temos pelo menos 1 linha disponível
        if (alturaDisponivelEstrutura < 1)
            alturaDisponivelEstrutura = 1;

        int inicioExibicao = Math.Max(0, visualStructure.Count - alturaDisponivelEstrutura);

        // Limpa a área da janela antes de escrever
        for (int i = 0; i < alturaDisponivelEstrutura; i++)
        {
            if (inicioJanelaEstrutura + i <= fimJanelaEstrutura)
            {
                Console.SetCursorPosition(3, inicioJanelaEstrutura + i);
                Console.Write(new string(' ', Console.WindowWidth - 8));
            }
        }

        // Exibe apenas as linhas que cabem na janela
        for (int i = 0; i < alturaDisponivelEstrutura && (inicioExibicao + i) < visualStructure.Count; i++)
        {
            int linhaY = inicioJanelaEstrutura + i;
            if (linhaY <= fimJanelaEstrutura)
            {
                Console.SetCursorPosition(3, linhaY);
                string linhaExibicao = visualStructure[inicioExibicao + i];
                if (linhaExibicao.Length > Console.WindowWidth - 8)
                {
                    linhaExibicao = linhaExibicao.Substring(0, Console.WindowWidth - 11) + "...";
                }
                Console.Write(linhaExibicao.PadRight(Console.WindowWidth - 8));
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

        // Calcula a posição dinamicamente baseada no layout
        int inicioStatus = Console.WindowHeight - 12; // Mais espaço para janela de status

        // Linha 1: Contadores de Pastas (+2 para pular título e borda superior)
        Console.SetCursorPosition(3, inicioStatus + 2);
        Console.Write($"[DIR] FOLDERS FOUND: {contadorPastas:000}".PadRight(Console.WindowWidth - 8));

        // Linha 2: Contadores de Arquivos
        Console.SetCursorPosition(3, inicioStatus + 3);
        Console.Write($"[FILE] FILES FOUND: {contadorArquivos:000}".PadRight(Console.WindowWidth - 8));

        // Linha 3: Total processado
        Console.SetCursorPosition(3, inicioStatus + 4);
        Console.Write($"[OK] PROCESSED: {itensProcessados:000} of {totalItens:000}".PadRight(Console.WindowWidth - 8));

        // Linha 4: Espaço em branco
        Console.SetCursorPosition(3, inicioStatus + 5);
        Console.Write("".PadRight(Console.WindowWidth - 8));

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
        // Calcula a posição dinamicamente baseada no layout
        int inicioStatus = Console.WindowHeight - 12;

        // Linha 1: Contadores de Pastas
        Console.SetCursorPosition(3, inicioStatus + 2);
        Console.Write($"[DIR] FOLDERS FOUND: {contadorPastas:000}".PadRight(Console.WindowWidth - 8));

        // Linha 2: Contadores de Arquivos
        Console.SetCursorPosition(3, inicioStatus + 3);
        Console.Write($"[FILE] FILES FOUND: {contadorArquivos:000}".PadRight(Console.WindowWidth - 8));

        // Linha 3: Total processado
        Console.SetCursorPosition(3, inicioStatus + 4);
        Console.Write($"[OK] PROCESSED: {itensProcessados:000} of {totalItens:000}".PadRight(Console.WindowWidth - 8));

        // Linha 4: Espaço em branco
        Console.SetCursorPosition(3, inicioStatus + 5);
        Console.Write("".PadRight(Console.WindowWidth - 8));

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
}