using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;

namespace Raveyard;

// copied from monogame's documentation
public class WAVStream: IDisposable
{
    // try not to change this
    // high buffer size = less stutter, more latency
    // low buffer size = more stutter, less latency
    // the balance seems to be at 50-200ms, anything lower than 20ms causes crackling (at least in my pc)
    private const int bufferSizeMs = 20;

    public DynamicSoundEffectInstance soundEffectInstance { get; private set; }

    public int bufferPos { get; private set; }
    public int bufferCount { get; private set; }
    public byte[] byteArray { get; private set; }

    private Stopwatch msClock = new Stopwatch(); // higher approximate precision
    private int audio_channels = 1;

    private void bufferNeeded(object sender, EventArgs args)
    {
        soundEffectInstance.SubmitBuffer(byteArray, bufferPos, bufferCount / 2);
        soundEffectInstance.SubmitBuffer(byteArray, bufferPos + bufferCount / 2, bufferCount / 2);

        bufferPos += bufferCount;
        if (bufferPos + bufferCount > byteArray.Length)
        {
            bufferPos = 0;
            StopMusic();
        }

        msClock.Restart();
    }

    public void loadFile(string filePath)
    {
        using WaveFileReader reader = new WaveFileReader(filePath);
        WaveFormat wavInfo = reader.WaveFormat;

        soundEffectInstance = new DynamicSoundEffectInstance(wavInfo.SampleRate, (AudioChannels)wavInfo.Channels);
        bufferCount = soundEffectInstance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(bufferSizeMs)) * wavInfo.Channels;

        MemoryStream memoryStream = new MemoryStream();
        reader.CopyTo(memoryStream);
        byteArray = memoryStream.ToArray();

        soundEffectInstance.BufferNeeded += new EventHandler<EventArgs>(bufferNeeded);
        audio_channels = wavInfo.Channels;

        soundEffectInstance.Volume = SETTINGS.AUDIO_MUSIC_MULT / 100f;
    }

    public void PlayMusic()
    {
        soundEffectInstance.Play();
        msClock.Start();
    }

    public void StopMusic()
    {
        bufferPos = 0;
        soundEffectInstance.Stop();
        msClock.Reset();
    }

    public double getBufferPositionMs()
    {
        if (soundEffectInstance == null) { return 0; }
        int pendingBuffers = (int) MathF.Ceiling(soundEffectInstance.PendingBufferCount / 2.0f);
        return (bufferPos-bufferCount*pendingBuffers)/bufferCount * ((double) bufferSizeMs * audio_channels)
        + Math.Min(msClock.ElapsedMilliseconds, bufferSizeMs*pendingBuffers);
    }

    public void Dispose()
    {
        StopMusic();
        soundEffectInstance.Dispose();
    }
}