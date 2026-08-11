using Terminal.Core.Interfaces;
using Android.Media;

namespace Terminal.Android.Services;

public class AndroidSoundService : ISoundService
{
    public void PlayBeep()
    {
        using var toneGen = new ToneGenerator(Stream.System, 100);
        toneGen.StartTone(Tone.PropBeep, 200);
    }
}