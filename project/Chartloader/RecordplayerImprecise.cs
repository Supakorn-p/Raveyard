using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Raveyard;

// uses DateTime instead

public class RecordPlayerImprecise
{
    private SoundEffect soundEffect;
    private double bpm;
    private double offset;

    private TimeSpan playbackTimerStart;

    public RecordPlayerImprecise(SoundEffect _SoundEffect, double _bpm, double _offset)
    {
        soundEffect = _SoundEffect;
        bpm = _bpm;
        offset = _offset;
    }

    public void Play()
    {
        soundEffect.Play();
        playbackTimerStart = TimeSpan.FromTicks(DateTime.Now.Ticks);
    }

    public double getCurrentBeattime()
    {
        double crotchet = 60.0 / bpm;
        double realTimeMs = TimeSpan.FromTicks(DateTime.Now.Ticks).Subtract(playbackTimerStart).TotalMilliseconds;
        double realPlaybackPos = (realTimeMs / 1000.0) - offset;

        // resyncBeattimeWithBuffer();
        return realPlaybackPos / crotchet;
    }
}