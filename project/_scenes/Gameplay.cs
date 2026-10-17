using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using Raveyard._scripts.Characters;
using Raveyard._scripts.Visuals;
using Raveyard._scripts.Visuals.EventObjects.Distractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Raveyard;

public class scGameplay : GameScreen
{
    private string currentFilename;
    private LEVELS_skinparams levelParameters;

    public scGameplay(Game game, string filenameToLoad, LEVELS_skinparams _levelParameters) : base(game)
    {
        currentFilename = filenameToLoad;
        levelParameters = _levelParameters;
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
        string fileName = LEVELS.GetLevelPathByName(_fileName);

        FileLoader file = new FileLoader();
        file.loadFile(fileName);

        timeline = new Timeline();

        File_Eventbaker baker = new File_Eventbaker();
        baker.Bake(ref timeline, file.eventList);

        foreach (TimelineEvent timelineEvent in file.eventList)
        {
            timeline.addEvent(timelineEvent.eventName, timelineEvent.beatTime, timelineEvent.parameters);
        }

        recordPlayer = new RecordPlayer(file.music, file.musicBPM, file.musicOffset);
        judgementSystem = new OrderJudgement();
        judgementSystem.SetBPM(file.musicBPM);
    }

    // GAME CONTENT GOES HERE VVV

    private Bag<EventObject> loadedEventObjects = new Bag<EventObject>(16);

    private void insertEventObjects()
    {
        loadedEventObjects.Add(new DialogTextBubble()); // dialog_bubble

        ResultsBubble resultsBubble = new ResultsBubble(); // display_score
        resultsBubble.SetScoreTracker(scoreTracker);
        loadedEventObjects.Add(resultsBubble);

        PasserBy passerBy = new PasserBy(); // dst_passerby
        loadedEventObjects.Add(passerBy);

        DemoPlayer demoPlayer = new DemoPlayer(); // demo_player
        loadedEventObjects.Add(demoPlayer);
    }

    private void initializeEventObjects()
    {
        foreach (EventObject eventObject in loadedEventObjects) 
        { 
            eventObject.Init(Content); 
            timeline.subscribeToEvent(eventObject.eventName, (EventParams eventParams) =>
            {
                eventObject.OnEvent(eventParams.data);
            });
        }
    }

    //private DrinkWanted drinkWanted;
    private void subscribeToEvents()
    {

        timeline.subscribeToEvent("start_order", (EventParams eventParams) => 
        { 
            judgementSystem.StartOrder(eventParams.beatTime);
            GameplaySoundLibrary.PlaySound("snd_cue_start");

            //order_box.StartOrder();
            Customer currentCustomer = customerQueue.AddCustomerToLine(0);
            currentCustomer.StartOrder();
            bartender.ActuallyTakingOrder();


            //drinkWanted = new DrinkWanted();
            //drinkWanted.CreateDrinkSprite();

            //customerQueue.customer.orderbox.orderedDrink = drinkWanted;
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

            Customer currentCustomer = customerQueue.getLastCustomer();
            currentCustomer.WaitForOrder();
        });

        timeline.subscribeToEvent("play_sound", (EventParams eventParams) => 
        { 
            Debug.WriteLine("received");
            GameplaySoundLibrary.PlaySound(eventParams.data[0]);
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

            //drinkWanted.SuccessInput();
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

    // Game Object Managers
    private CustomerQueue customerQueue;

    // text
    private SpriteFont font_antonSc;

    public override void LoadContent()
    {
        base.LoadContent();
        loadChart(currentFilename);
        scoreTracker = new ScoreTracker();

        bartender = new Bartender();

        customerTextures = new TextureCollection(Content);
        customerQueue = new CustomerQueue(customerTextures);
        //customerQueue.InitializeTexture("customer_sprsheet", new Vector2(551, 581));
        foreach (CustomerVariantData customer in levelParameters.customerVariants)
        {
            customerQueue.InitializeTexture(customer);
        }


        insertEventObjects();
        initializeEventObjects();
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
        Content.Load<Texture2D>("bg_halloween "), new Vector2(1280, 720), Vector2.Zero);
        background.layer = -2;

        bar_counter = new SpriteObject("bar_counter", Content.Load<Texture2D>("counter_halloween"), 
        new Vector2(1280, 720), Vector2.Zero);
        bar_counter.layer = -1;

        // Load Characters

        bartender.bartender = new SpriteObject("bartender", Content.Load<Texture2D>("bartender_sprsheet_2"), 
        new Vector2(430, 486), bartender.position); //.bartender is the SpriteObject in that class
        bartender.bartender.active = true;

        bartender.eyes = new SpriteObject("bartender_eyes", Content.Load<Texture2D>("bartender_eyes_sprsheet"),
        new Vector2(430, 486), bartender.position);
        bartender.eyes.layer = bartender.bartender.layer + 1;
        bartender.eyes.active = true;

        bartender.BartenderInitialize();


        // Load Gameplay Objects
        OrderBox.InitializeOrderBox(Content.Load<Texture2D>("box_halloween"), Content.Load<Texture2D>("inskeys_sprsheet"));
        DrinkWanted.InitializeWantedDrink(Content.Load<Texture2D>("drinks_sprsheet"));

        susie.active = true;
        background.active = true;
        bar_counter.active = true;
    }

    public override void Update(GameTime gameTime)
    {
        timeline.Update(recordPlayer.getCurrentBeattime());
        judgementSystem.Update(timeline.beatTimeNeedle);

        foreach (DrawableObject drawObj in Drawablekeeper.getActiveObjs())
        {
            drawObj.Update(gameTime);
        }

        foreach (EventObject eventObject in loadedEventObjects) { eventObject.Update(gameTime); }

        // bleh
        if (KeyboardExtended.GetState().WasKeyPressed(SETTINGS.keybind_back))
        {
            ScreenManager.ReplaceScreen(new scMenu(Game));
        }
    }
    public override void Draw(GameTime gameTime)
    {
        //GraphicsDevice.Clear(Color.Green);

        _spriteBatch.Begin(
            transformMatrix: _camera.GetViewMatrix(), 
            samplerState: SamplerState.PointClamp, 
            sortMode: SpriteSortMode.BackToFront,
            blendState: BlendState.NonPremultiplied);

        foreach (DrawableObject drawObj in Drawablekeeper.getActiveObjs())
        {
            drawObj.Draw(_spriteBatch);
        }

        _spriteBatch.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        recordPlayer.Stop();
        recordPlayer.Dispose();
        customerTextures.UnloadAll();
        sfxlib.UnloadAll();
        Drawablekeeper.FreeAll();
    }
}