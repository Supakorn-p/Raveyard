using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.ViewportAdapters;
using Raveyard._scripts.Characters;
using Raveyard._scripts.Visuals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            //Debug.Write("\nOrder: ");

            order_box.StartOrder();
        });

        timeline.subscribeToEvent("press", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.press);
            beep.Play(); 
            //Debug.Write("[_] ");

            order_box.InstructionAdded(InputType.press);
        });

        timeline.subscribeToEvent("left", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.left);
            beep.Play(); 
            //Debug.Write("<- ");

            order_box.InstructionAdded(InputType.left);
        });

        timeline.subscribeToEvent("right", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.right);
            beep.Play(); 
            //Debug.Write("-> ");

            order_box.InstructionAdded(InputType.right); 
        });

        timeline.subscribeToEvent("end_order", (EventParams eventParams) => 
        { 
            judgementSystem.StopOrderAndListen(eventParams.beatTime);
            Debug.WriteLine("\n!!");

            //order_box.EndOrder();
        });

        judgementSystem.inputResult += (JudgementResult result) =>
        {
            beep_player.Play();
            bartender.OnInput();
            if (result == JudgementResult.none) { return; } // misinputs, usually

            order_box.RemoveInstruction();
            if (result == JudgementResult.miss) { beep_missed.Play(); return; }

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

    // Backgrounds
    private SpriteObject susie;
    private SpriteObject background;
    private SpriteObject bar_counter;

    // Game Objects
    private Bartender bartender;
    private OrderBox order_box;

    public override void LoadContent()
    {
        base.LoadContent();
        loadChart(currentFilename);
        judgementSystem = new OrderJudgement();

        bartender = new Bartender();
        order_box = new OrderBox();
        subscribeToEvents();

        beep = Content.Load<SoundEffect>("beep");
        beep_player = Content.Load<SoundEffect>("inputbeep");
        beep_success = Content.Load<SoundEffect>("inputsuccess");
        beep_missed = Content.Load<SoundEffect>("inputmissed");
        recordPlayer.Play();

        _spriteBatch = new SpriteBatch(GraphicsDevice);


        // SECRET SUSIE ADDITION NO ONE WILL EVER KNOW
        susie = new SpriteObject("susie", 
        Content.Load<Texture2D>("placeholder"), new Vector2(640, 640),
        Vector2.Zero);

        background = new SpriteObject("background",
        Content.Load<Texture2D>("BG"), new Vector2(1280, 720), Vector2.Zero);

        bar_counter = new SpriteObject("bar_counter", Content.Load<Texture2D>("Bar-counter"), 
        new Vector2(1280, 720), Vector2.Zero);

        // Load Characters

        bartender.bartender = new SpriteObject("bartender", Content.Load<Texture2D>("bartender_sprsheet_1"), 
        new Vector2(430, 486), bartender.position); //.bartender is the SpriteObject in that class
        bartender.bartender.active = true;
        bartender.BartenderInitialize();


        // Load Gameplay Objects

        order_box.order_box = new SpriteObject("order_box", Content.Load<Texture2D>("Dialogue-Box"), 
        new Vector2(800, 400), order_box.position);
        order_box.order_box.active = true;
        order_box.InitializeOrderBox(Content.Load<Texture2D>("instkeys_sprsheet"));

        susie.active = true;
        background.active = true;
        bar_counter.active = true;
    }

    public override void Update(GameTime gameTime)
    {
        judgementSystem.Update(timeline.beatTimeNeedle);
        timeline.Update(recordPlayer.getCurrentBeattime());

        foreach (SpriteObject spriteObj in Spritekeeper.getActiveObjs())
        {
            spriteObj.animatedSprite.Update(gameTime);
            spriteObj.tweener.Update(gameTime.GetElapsedSeconds());
        }
    }
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Green);

        _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

        foreach (SpriteObject spriteObj in Spritekeeper.getActiveObjs())
        {
            Vector2 trueScale = new Vector2(spriteObj.animatedSprite.Size.X, spriteObj.animatedSprite.Size.Y) * spriteObj.scale;
            Vector2 finalOffset = new Vector2(spriteObj.anchor.X * trueScale.X,
            spriteObj.anchor.Y * trueScale.Y);
            _spriteBatch.Draw(spriteObj.animatedSprite, spriteObj.position - finalOffset, spriteObj.rotation, spriteObj.scale);
        }

        _spriteBatch.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        recordPlayer.Stop();
    }
}