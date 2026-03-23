using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Terminal.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (!(OperatingSystem.IsAndroid() || OperatingSystem.IsBrowser())) 
            return;
        
        var topLevel = TopLevel.GetTopLevel(this);
        var insetsManager = topLevel?.InsetsManager;

        if (insetsManager == null) 
            return;
            
        insetsManager.DisplayEdgeToEdge = true;
        insetsManager.IsSystemBarVisible = false;
    }
}