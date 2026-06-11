using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Interactivity;
using Terminal.ViewModels.Pages;

namespace Terminal.Views.Pages;

public partial class SettingsPageView : UserControl
{
    private IInputPane? _inputPane;
    private IInsetsManager? _insetsManager;
    
    public SettingsPageView()
    {
        InitializeComponent();
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        var topLevel = TopLevel.GetTopLevel(this)!;
        _inputPane = topLevel.InputPane;
        _insetsManager = topLevel.InsetsManager;

        if (_inputPane != null)
        {
            _inputPane.StateChanged += InputPane_StateChanged;
        }
    }
    
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (_inputPane != null)
        {
            _inputPane.StateChanged -= InputPane_StateChanged;
        }
    }
    
    private void InputPane_StateChanged(object? sender, InputPaneStateEventArgs e)
    {
        if (DataContext is not SettingsPageViewModel model || _inputPane == null || _insetsManager == null) 
            return;
        
        var safeAreaPadding = _insetsManager.SafeAreaPadding;
        
        var occludedHeight = _inputPane.OccludedRect.Height;
        
        model.SafeArea = new Thickness(
            safeAreaPadding.Left,
            safeAreaPadding.Top,
            safeAreaPadding.Right,
            occludedHeight
        );
    }
}