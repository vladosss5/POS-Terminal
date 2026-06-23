using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace Terminal.Android;

[Activity(
    Label = "SncTerminal",
    Theme = "@style/Theme.AppCompat.Light.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}