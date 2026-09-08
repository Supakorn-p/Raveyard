using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;

namespace Raveyard;

public class scGameplay : GameScreen
{
    private string currentFilename;

    public scGameplay(Game game, string filenameToLoad) : base(game)
    {
        currentFilename = filenameToLoad;
    }
    
    private SpriteBatch _spriteBatch;
    private OrthographicCamera _camera;

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

    public override void Initialize()
    {
        base.Initialize();
        Vector2 res = new Vector2(1280, 720);
        ViewportAdapter viewport = new BoxingViewportAdapter(Game.Window, GraphicsDevice, (int)res.X, (int)res.Y);
        _camera = new OrthographicCamera(viewport);
        _camera.Position = res / -2;
    }

    private SpriteObject susie;
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

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        susie = new SpriteObject("susie", 
        Content.Load<Texture2D>("placeholder"), new Rectangle(0, 0, 640, 640),
        Vector2.Zero);

        susie.active = true;
    }

    public override void Update(GameTime gameTime)
    {
        timeline.Update(recordPlayer.getCurrentBeattime());
        judgementSystem.Update(timeline.beatTimeNeedle);

        foreach (SpriteObject spriteObj in Spritekeeper.getActiveObjs())
        {
            spriteObj.animatedSprite.Update(gameTime);
        }
    }
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Green);

        _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

        foreach (SpriteObject spriteObj in Spritekeeper.getActiveObjs())
        {
            Vector2 finalOffset = new Vector2(spriteObj.anchor.X * spriteObj.animatedSprite.Size.X,
            spriteObj.anchor.Y * spriteObj.animatedSprite.Size.Y);
            _spriteBatch.Draw(spriteObj.animatedSprite, spriteObj.position - finalOffset, spriteObj.rotation);
        }

        _spriteBatch.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        recordPlayer.Stop();
    }
}