using System;
using Terminal.Core.Interfaces;
using Android.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using Terminal.Core.Enums;

namespace Terminal.Android.Services;

/// <summary>
/// Android реализация сервиса воспроизведения звуков.
/// </summary>
public class AndroidSoundService : ISoundService
{
    /// <summary>
    /// Сервис логирования.
    /// </summary>
    private readonly ILogger<AndroidSoundService> _logger;

    /// <summary>
    /// Путь к аудиофайлу нажатия на кнопку.
    /// </summary>
    private const string ButtonSoundPath = "Sounds/button-click.wav";

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidSoundService(ILogger<AndroidSoundService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public void PlayBeep()
    {
        using var toneGen = new ToneGenerator(Stream.System, 100);
        toneGen.StartTone(Tone.PropBeep, 200);
    }

    /// <inheritdoc/>
    public void PlaySound(SoundType soundType)
    {
        var deviceManufacturer = DeviceInfo.Current.Manufacturer;
        if (deviceManufacturer == "alps" && soundType == SoundType.Button)
            return;
            
        var fileName = soundType switch
        {
            SoundType.Button => ButtonSoundPath,
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(fileName))
            return;

        try
        {
            var player = new MediaPlayer();
            var context = global::Android.App.Application.Context;
            var fd = context.Assets!.OpenFd(fileName);
            player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
            player.Prepare();
            player.Start();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e.InnerException);
        }
    }
}