using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Raveyard;

// copied from monogame's documentation
public class WAVStream
{
    // try not to change this
    // high buffer size = less stutter, more latency
    // low buffer size = more stutter, less latency
    // the balance seems to be at 50-200ms, anything lower than 20ms causes crackling (at least in my pc)
    private const int bufferSizeMs = 50;

    public DynamicSoundEffectInstance soundEffectInstance { get; private set; }

    public int bufferPos { get; private set; }
    public int bufferCount { get; private set; }
    public byte[] byteArray { get; private set; }

    private Stopwatch msClock = new Stopwatch(); // higher approximate precision

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
        Stream wavfileStream = File.OpenRead(filePath);
        BinaryReader reader = new BinaryReader(wavfileStream);

        int chunkID = reader.ReadInt32();
        int fileSize = reader.ReadInt32();
        int riffType = reader.ReadInt32();
        int fmtID = reader.ReadInt32();
        int fmtSize = reader.ReadInt32();
        int fmtCode = reader.ReadInt16();
        int channels = reader.ReadInt16();
        int sampleRate = reader.ReadInt32();
        int fmtAvgBPS = reader.ReadInt32();
        int fmtBlockAlign = reader.ReadInt16();
        int bitDepth = reader.ReadInt16();

        if (fmtSize == 18)
        {
            int fmtExtraSize = reader.ReadInt16();
            reader.ReadBytes(fmtExtraSize);
        }

        int dataID = reader.ReadInt32();
        int dataSize = reader.ReadInt32();

        soundEffectInstance = new DynamicSoundEffectInstance(sampleRate, (AudioChannels)channels);
        bufferCount = soundEffectInstance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(bufferSizeMs));
        byteArray = reader.ReadBytes(dataSize);

        soundEffectInstance.BufferNeeded += new EventHandler<EventArgs>(bufferNeeded);
    }

    public void PlayMusic()
    {
        soundEffectInstance.Play();
        msClock.Start();
    }

    public void StopMusic()
    {
        soundEffectInstance.Stop();
        msClock.Reset();
    }

    public double getBufferPositionMs()
    {
        if (soundEffectInstance == null) { return 0; }
        int pendingBuffers = (int) MathF.Ceiling(soundEffectInstance.PendingBufferCount / 2.0f);
        return (bufferPos-bufferCount*pendingBuffers)/bufferCount * (double) bufferSizeMs
        + Math.Min(msClock.ElapsedMilliseconds, bufferSizeMs*pendingBuffers);
    }
}