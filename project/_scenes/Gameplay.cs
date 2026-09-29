using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using Raveyard._scripts.Characters;
using Raveyard._scripts.Visuals;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

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
    private ScoreTracker scoreTracker;

    private GameplaySoundLibrary sfxlib;
    private TextureCollection customerTextures;

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

            //order_box.StartOrder();
            Customer currentCustomer = customerQueue.AddCustomerToLine(0);
            currentCustomer.StartOrder();
            bartender.SetAnimation("bar_idle");  
        });

        timeline.subscribeToEvent("press", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.press);
            GameplaySoundLibrary.PlaySound("snd_cue_placeholder_press"); // TODO: replace by calling the customer class

            //order_box.InstructionAdded(InputType.press);
            Customer currentCustomer = customerQueue.getLastCustomer();
            currentCustomer.AddToOrder(InputType.press);
        });

        timeline.subscribeToEvent("left", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.left);
            GameplaySoundLibrary.PlaySound("snd_cue_placeholder_left"); // TODO: replace by calling the customer class

            //order_box.InstructionAdded(InputType.left);
            Customer currentCustomer = customerQueue.getLastCustomer();
            currentCustomer.AddToOrder(InputType.left);
        });

        timeline.subscribeToEvent("right", (EventParams eventParams) => 
        { 
            judgementSystem.AddInputToOrder(eventParams.beatTime, InputType.right);
            GameplaySoundLibrary.PlaySound("snd_cue_placeholder_right"); // TODO: replace by calling the customer class

            //order_box.InstructionAdded(InputType.right); 
            Customer currentCustomer = customerQueue.getLastCustomer();
            currentCustomer.AddToOrder(InputType.right);
        });

        timeline.subscribeToEvent("end_order", (EventParams eventParams) => 
        { 
            judgementSystem.StopOrderAndListen(eventParams.beatTime);
            GameplaySoundLibrary.PlaySound("snd_cue_end");
        });

        judgementSystem.inputResult += ((JudgementResult result, InputType input) tuple) =>
        {
            bartender.OnInputResult(tuple.result, tuple.input);

            if (tuple.result == JudgementResult.none) { return; } // misinputs, usually

            //order_box.RemoveInstruction(tuple.result);
            Customer currentCustomer = customerQueue.getFirstCustomer();
            currentCustomer?.ProcessOrder(tuple.result);
            scoreTracker.AddJudgement(tuple.result);
            Debug.WriteLine(Math.Floor(scoreTracker.GetFinalPercentage()));
        };

        timeline.subscribeToEvent("ENDCHART", (EventParams eventParams) => 
        { 
            resultsBubble.DisplayScore((int) Math.Floor(scoreTracker.GetFinalPercentage()));
        });
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
    private placeholder_ResultsBubble resultsBubble;

    // Game Object Managers
    private CustomerQueue customerQueue;

    // text
    private SpriteFont font_antonSc;

    public override void LoadContent()
    {
        base.LoadContent();
        loadChart(currentFilename);
        judgementSystem = new OrderJudgement();
        scoreTracker = new ScoreTracker();

        bartender = new Bartender();
        resultsBubble = new placeholder_ResultsBubble();

        customerTextures = new TextureCollection(Content);
        customerQueue = new CustomerQueue(customerTextures);
        customerQueue.InitializeTexture("customer_halloween1_sprsheet", new Vector2(444, 483));

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

        font_antonSc = Content.Load<SpriteFont>("anton_sc");
        

        // SECRET SUSIE ADDITION NO ONE WILL EVER KNOW
        susie = new SpriteObject("susie", 
        Content.Load<Texture2D>("placeholder"), new Vector2(640, 640),
        Vector2.Zero);
        susie.layer = -99;

        background = new SpriteObject("background",
        Content.Load<Texture2D>("BG"), new Vector2(1280, 720), Vector2.Zero);
        background.layer = -2;

        bar_counter = new SpriteObject("bar_counter", Content.Load<Texture2D>("Bar-counter"), 
        new Vector2(1280, 720), Vector2.Zero);
        bar_counter.layer = -1;

        // Load Characters

        bartender.bartender = new SpriteObject("bartender", Content.Load<Texture2D>("bartender_sprsheet_1"), 
        new Vector2(430, 486), bartender.position); //.bartender is the SpriteObject in that class
        bartender.bartender.active = true;
        bartender.BartenderInitialize();


        // Load Gameplay Objects

        resultsBubble.Init(Content.Load<Texture2D>("Dialogue-Box"), font_antonSc);
        OrderBox.InitializeOrderBox(Content.Load<Texture2D>("Dialogue-Box"), Content.Load<Texture2D>("instkeys_sprsheet"));

        susie.active = true;
        background.active = true;
        bar_counter.active = true;
    }

    public override void Update(GameTime gameTime)
    {
        timeline.Update(recordPlayer.getCurrentBeattime());
        judgementSystem.Update(timeline.beatTimeNeedle);

        foreach (SpriteObject spriteObj in Spritekeeper.spriteObjects.ToArray())
        {
            spriteObj.animatedSprite.Update(gameTime);
            spriteObj.tweener.Update(gameTime.GetElapsedSeconds());
        }

        // bleh
        resultsBubble.Update(gameTime);
        if (KeyboardExtended.GetState().WasKeyPressed(Keys.R))
        {
            ScreenManager.ReplaceScreen(new scGameplay(Game, "peakuniku"));
        }
    }
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Green);

        _spriteBatch.Begin(
            transformMatrix: _camera.GetViewMatrix(), 
            samplerState: SamplerState.PointClamp, 
            sortMode: SpriteSortMode.BackToFront);

        foreach (SpriteObject spriteObj in Spritekeeper.getActiveObjs())
        {
            float _rotation = spriteObj.rotation/180f * MathF.PI;

            spriteObj.SetSpriteValues();

            _spriteBatch.Draw(spriteObj.animatedSprite, spriteObj.position, _rotation, spriteObj.scale);
        }

        foreach (TextObject textObj in Textkeeper.getActiveObjs())
        {
            float _rotation = textObj.rotation/180f * MathF.PI;
            _spriteBatch.DrawString(textObj.font, textObj.text, textObj.position, 
            new Color(textObj.color, textObj.alpha), _rotation, textObj.GetRawOrigin(), textObj.scale,
            SpriteEffects.None, textObj.LayerToDepth());
        }

        _spriteBatch.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        recordPlayer.Stop();
        customerTextures.UnloadAll();
        sfxlib.UnloadAll();
        Spritekeeper.FreeAll();
        Textkeeper.FreeAll();
    }
}