using System.Windows.Input;

namespace Terminal.ViewModels.Items;

public class MainMenuItemModel
{
    public string Title { get; set; }
    public ICommand? Command { get; set; }
    public object Parameter { get; set; }
}