using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Raveyard;

// monogame's gameTime is cool but its also unreliable so this is a compromise

public class RecordPlayer : IDisposable
{
    private WAVStream wavStream;
    public double bpm {get; private set;}
    public double offset {get; private set;}

    public RecordPlayer(WAVStream _soundEffect, double _bpm, double _offset)
    {
        wavStream = _soundEffect;
        bpm = _bpm;
        offset = _offset;
    }

    public void Play()
    {
        wavStream.PlayMusic();
    }

    public void Stop()
    {
        wavStream.StopMusic();
    }

    public double getCurrentBeattime()
    {
        double crotchet = 60.0 / bpm;
        double realPlaybackPos = getCurrentRealTime();
        
        return realPlaybackPos / crotchet;
    }

    public double getCurrentRealTime()
    {
        double realTimeMs = wavStream.getBufferPositionMs();
        return (realTimeMs / 1000.0) - offset;
    }

    public void Dispose()
    {
        wavStream.Dispose();
    }
}