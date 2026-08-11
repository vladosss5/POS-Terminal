using System;
using System.Diagnostics;
using System.Media;
using Avalonia.Platform;
using Microsoft.Extensions.Logging;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.Desktop.Services;

/// <summary>
/// Desktop реализация сервиса воспроизведения звуков.
/// </summary>
public class DesktopSoundService : ISoundService
{
    /// <summary>
    /// Сервис логирования.
    /// </summary>
    private readonly ILogger<DesktopSoundService> _logger;

    /// <summary>
    /// Путь к файлу писка на Linux.
    /// </summary>
    private const string LinuxBeepFilePath = "/usr/share/sounds/freedesktop/stereo/bell.oga";

    /// <summary>
    /// Uri аудиофайла со звуком нажатия кнопки.
    /// </summary>
    private const string ButtonSoundUri = "avares://Terminal/Assets/Sounds/button-click.wav";

    /// <summary>
    /// Конструктор.
    /// </summary>
    public DesktopSoundService(ILogger<DesktopSoundService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public void PlayBeep()
    {
        if (OperatingSystem.IsWindows())
        {
            Console.Beep(3000, 100);
        }
        else if (OperatingSystem.IsLinux())
        {
            Process.Start("paplay", LinuxBeepFilePath);
        }
    }

    /// <inheritdoc/>
    public void PlaySound(SoundType soundType)
    {
        var uri = soundType switch
        {
            SoundType.Button => ButtonSoundUri,
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(uri))
            return;

        try
        {
            if (OperatingSystem.IsWindows())
            {
                var stream = AssetLoader.Open(new Uri(uri));
                var player = new SoundPlayer(stream);
                player.Play();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
        }
    }
}