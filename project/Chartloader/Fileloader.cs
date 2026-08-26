using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Raveyard;

public class FileLoader
{
    public List<TimelineEvent> eventList { get; private set; } = new List<TimelineEvent>();
    public WAVStream music { get; private set; }

    public double musicBPM { get; private set; }
    public double musicOffset { get; private set; }

    private void transcribeData(string[] line)
    {
        string prefix = line[0];

        EventParams eventParams = new EventParams(line[1]);
        string[] parameters = eventParams.data_raw;

        if (prefix == "bpm") { musicBPM = double.Parse(parameters[0]); return; }
        if (prefix == "offset") { musicOffset = double.Parse(parameters[0]); return; }

        TimelineEvent newEvent = new TimelineEvent
        {
            eventName = prefix,
            parameters = eventParams,
            beatTime = double.Parse(parameters[0])
        };

        eventList.Add(newEvent);
    }

    private void loadSong(string filePath)
    {
        if (music != null) { return; }
        WAVStream newStream = new WAVStream();
        newStream.loadFile(filePath);
        music = newStream;
    }

    public void loadFile(string _filePath)
    {
        string raveyardfilePath = Path.ChangeExtension(_filePath, "raveyard");
        using (StreamReader sr = new StreamReader(raveyardfilePath))
        {
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line)) { continue; }
                transcribeData(line.Split(":"));
            }
        }

        loadSong(Path.ChangeExtension(_filePath, "wav"));
    }
}