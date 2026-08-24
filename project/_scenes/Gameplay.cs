using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Raveyard;

public class scGameplay : GameScreen
{
    private string currentFilename;

    public scGameplay(Game game, string filenameToLoad) : base(game)
    {
        currentFilename = filenameToLoad;
    }

    private RecordPlayer recordPlayer;
    private Timeline timeline;
    private void loadChart(string _fileName)
    {
        string fileName = Path.Combine(Directory.GetCurrentDirectory(), @"_charts\", _fileName);

        FileLoader file = new FileLoader();
        file.loadFile(fileName);

        timeline = new Timeline();
        foreach (TimelineEvent timelineEvent in file.eventList)
        {
            timeline.addEvent(timelineEvent.eventName, timelineEvent.beatTime, timelineEvent.parameters);
        }

        recordPlayer = new RecordPlayer(file.music, file.musicBPM, file.musicOffset);
    }

    SoundEffect beep;
    public override void LoadContent()
    {
        base.LoadContent();
        loadChart(currentFilename);
        beep = Content.Load<SoundEffect>("beep");

        timeline.subscribeToEvent("start_order", (_) => { beep.Play(); });
        timeline.subscribeToEvent("press", (_) => { beep.Play(); });
        timeline.subscribeToEvent("left", (_) => { beep.Play(); });
        timeline.subscribeToEvent("right", (_) => { beep.Play(); });
        timeline.subscribeToEvent("end_order", (_) => { beep.Play(); });

        recordPlayer.Play();
    }

    public override void Update(GameTime gameTime)
    {
        timeline.Update(recordPlayer.getCurrentBeattime());
    }
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Green);
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        recordPlayer.Stop();
    }
}