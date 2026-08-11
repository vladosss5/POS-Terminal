using Terminal.Core.Enums;

namespace Terminal.Core.Interfaces;

/// <summary>
/// Сервис воспроизведения звуков.
/// </summary>
public interface ISoundService
{
    /// <summary>
    /// Воспроизвести писк.
    /// </summary>
    public void PlayBeep();

    /// <summary>
    /// Воспроизвести типовое аудио.
    /// </summary>
    /// <param name="soundType">Тип аудио.</param>
    public void PlaySound(SoundType soundType);
}