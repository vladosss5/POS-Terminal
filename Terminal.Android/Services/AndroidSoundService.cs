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
    private readonly ILoggingService _logger;

    /// <summary>
    /// Путь к аудиофайлу нажатия на кнопку.
    /// </summary>
    private const string ButtonSoundPath = "Sounds/button-click.wav";

    /// <summary>
    /// Путь к аудиофайлу успешное выполнение операции.
    /// </summary>
    private const string SuccessSoundPath = "Sounds/success.wav";
    
    /// <summary>
    /// Путь к аудиофайлу ошибка.
    /// </summary>
    private const string ErrorSoundPath = "Sounds/error.wav";

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AndroidSoundService(ILoggingService logger)
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
            SoundType.Success => SuccessSoundPath,
            SoundType.Error => ErrorSoundPath,
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
            _logger.LogError($"Ошибка воспроизведения звука\n{e.Message}\n{e.InnerException}");
        }
    }
}