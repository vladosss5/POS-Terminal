using System;
using System.Diagnostics;
using Terminal.Core.Interfaces;

namespace Terminal.Desktop.Services;

public class DesktopSoundService : ISoundService
{
    public void PlayBeep()
    {
        if (OperatingSystem.IsWindows())
        {
            // SystemSounds.Beep.Play();
            // SystemSounds.Asterisk.Play();
            Console.Beep(3000, 100);
        }
        else if (OperatingSystem.IsLinux())
        {
            Process.Start("paplay", "/usr/share/sounds/freedesktop/stereo/bell.oga");
        }
    }
}