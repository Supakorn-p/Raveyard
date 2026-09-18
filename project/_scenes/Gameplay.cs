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
    private GameplaySoundLibrary sfxlib;

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
            GameplaySoundLibrary.PlaySound("snd_cue_start");

            order_box.StartOrder();
            bartender.SetAnimation("bar_idle");  
        });

        timeline.subscribeToEvent("press", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.press);
            GameplaySoundLibrary.PlaySound("snd_cue_placeholder_press"); // TODO: replace by calling the customer class

            order_box.InstructionAdded(InputType.press);
        });

        timeline.subscribeToEvent("left", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.left);
            GameplaySoundLibrary.PlaySound("snd_cue_placeholder_left"); // TODO: replace by calling the customer class

            order_box.InstructionAdded(InputType.left);
        });

        timeline.subscribeToEvent("right", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.right);
            GameplaySoundLibrary.PlaySound("snd_cue_placeholder_right"); // TODO: replace by calling the customer class

            order_box.InstructionAdded(InputType.right); 
        });

        timeline.subscribeToEvent("end_order", (EventParams eventParams) => 
        { 
            judgementSystem.StopOrderAndListen(eventParams.beatTime);
            GameplaySoundLibrary.PlaySound("snd_cue_end");

            //order_box.EndOrder();
        });

        judgementSystem.inputResult += ((JudgementResult result, InputType input) tuple) =>
        {
            bartender.OnInputResult(tuple.result, tuple.input);

            if (tuple.result == JudgementResult.none) { return; } // misinputs, usually
            order_box.RemoveInstruction(tuple.result);
        };

        // game object events

        order_box.inputsExhausted += (bool perfect) =>
        {
            if (perfect)
            {
                bartender.SetAnimation("bar_finish");
            }
        };
    }

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

        recordPlayer.Play();

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // SFX
        sfxlib = new GameplaySoundLibrary(Content);

        //sfxlib.LoadSound("beep");
        sfxlib.LoadSound("inputbeep");
        sfxlib.LoadSound("inputmissed");
        sfxlib.LoadSound("snd_input_glassclink");
        sfxlib.LoadSound("snd_input_shakeleft");
        sfxlib.LoadSound("snd_input_shakeright");

        sfxlib.LoadSound("snd_cue_start");
        sfxlib.LoadSound("snd_cue_placeholder_press");
        sfxlib.LoadSound("snd_cue_placeholder_left");
        sfxlib.LoadSound("snd_cue_placeholder_right");
        sfxlib.LoadSound("snd_cue_end");
        
        sfxlib.LoadSound("snd_result_cashregister");
        

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


            _spriteBatch.Draw(spriteObj.animatedSprite, spriteObj.position - finalOffset, spriteObj.rotation/180f * MathF.PI, spriteObj.scale);
        }

        _spriteBatch.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        recordPlayer.Stop();
    }
}