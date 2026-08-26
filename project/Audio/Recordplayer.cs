using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Raveyard;

// monogame's gameTime is cool but its also unreliable so this is a compromise

public class RecordPlayer
{
    private WAVStream wavStream;
    //private DynamicSoundEffectInstance soundEffect;
    private double bpm;
    private double offset;

    //private Stopwatch playbackTimer;
    //private double bufferOffset;

    public RecordPlayer(WAVStream _soundEffect, double _bpm, double _offset)
    {
        wavStream = _soundEffect;
        //soundEffect = _soundEffect.soundEffectInstance;
        bpm = _bpm;
        offset = _offset;
    }

    public void Play()
    {
        wavStream.PlayMusic();
        //soundEffect.Play();
        //playbackTimer = new Stopwatch();
        //playbackTimer.Start();
        //bufferOffset = resyncBeattimeWithBuffer().TotalMilliseconds / 1000.0;
    }

    public void Stop()
    {
        wavStream.StopMusic();
    }

    // private TimeSpan lastbufferTime = TimeSpan.Zero;

    // private TimeSpan resyncBeattimeWithBuffer()
    // {
    //     TimeSpan bufferTime = MediaPlayer.PlayPosition;
    //     if (lastbufferTime == bufferTime) { return lastbufferTime; }
    //     lastbufferTime = MediaPlayer.PlayPosition;
    //     playbackTimer.Restart();

    //     return lastbufferTime;
    // }

    public double getCurrentBeattime()
    {
        double crotchet = 60.0 / bpm;
        //double realTimeMs = playbackTimer.ElapsedMilliseconds;
        double realTimeMs = wavStream.getBufferPositionMs();
        double realPlaybackPos = (realTimeMs / 1000.0) - offset;

        // resyncBeattimeWithBuffer();
        return realPlaybackPos / crotchet;
    }
}