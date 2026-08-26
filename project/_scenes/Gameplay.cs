using System;
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
    private OrderJudgement judgementSystem;

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

    // GAME CONTENT GOES HERE VVV

    private void subscribeToEvents()
    {
        timeline.subscribeToEvent("start_order", (EventParams eventParams) => 
        { 
            judgementSystem.StartOrder(eventParams.beatTime);
            Debug.Write("\nOrder: ");
        });

        timeline.subscribeToEvent("press", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.press);
            beep.Play(); 
            Debug.Write("[_] ");
        });

        timeline.subscribeToEvent("left", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.left);
            beep.Play(); 
            Debug.Write("<- ");
        });

        timeline.subscribeToEvent("right", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.right);
            beep.Play(); 
            Debug.Write("-> ");
        });

        timeline.subscribeToEvent("end_order", (EventParams eventParams) => 
        { 
            judgementSystem.StopOrderAndListen(eventParams.beatTime);
            Debug.WriteLine("\n!!");
        });

        judgementSystem.inputResult += (JudgementResult result) =>
        {
            if (result == JudgementResult.miss) { beep_missed.Play(); return; }
            beep_player.Play();
            Debug.WriteLine(result);

            if (result == JudgementResult.perfect)
            {
                beep_success.Play();
            }
        };
    }

    SoundEffect beep;
    SoundEffect beep_player;
    SoundEffect beep_success;
    SoundEffect beep_missed;

    public override void LoadContent()
    {
        base.LoadContent();
        loadChart(currentFilename);
        judgementSystem = new OrderJudgement();
        subscribeToEvents();

        beep = Content.Load<SoundEffect>("beep");
        beep_player = Content.Load<SoundEffect>("inputbeep");
        beep_success = Content.Load<SoundEffect>("inputsuccess");
        beep_missed = Content.Load<SoundEffect>("inputmissed");
        recordPlayer.Play();
    }

    public override void Update(GameTime gameTime)
    {
        timeline.Update(recordPlayer.getCurrentBeattime());
        judgementSystem.Update(timeline.beatTimeNeedle);
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