namespace DesignProjectStructure.Models;

/// <summary>
/// Representa uma opção do menu principal
/// </summary>
public class MenuOption
{
    /// <summary>
    /// Identificador único da opção
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Título exibido no menu
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada da funcionalidade
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Ícone/emoji para exibir no menu
    /// </summary>
    public string Icon { get; set; } = "⚙️";

    /// <summary>
    /// Indica se a opção está habilitada
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Indica se é uma opção especial (Exit, Help, etc.)
    /// </summary>
    public bool IsSpecial { get; set; } = false;

    /// <summary>
    /// Ação a ser executada quando a opção é selecionada
    /// </summary>
    public Action? Action { get; set; }

    /// <summary>
    /// Tecla de atalho (opcional)
    /// </summary>
    public ConsoleKey? ShortcutKey { get; set; }

    /// <summary>
    /// Construtor vazio
    /// </summary>
    public MenuOption() { }

    /// <summary>
    /// Construtor completo
    /// </summary>
    public MenuOption(string id, string title, string description, string icon = "⚙️", bool isEnabled = true, Action? action = null)
    {
        Id = id;
        Title = title;
        Description = description;
        Icon = icon;
        IsEnabled = isEnabled;
        Action = action;
    }
}