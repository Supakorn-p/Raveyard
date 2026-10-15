using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Collections;
using System.Linq;

namespace Raveyard;

public class scMenu : GameScreen
{
    public scMenu(Game game) : base(game) {}

    private SpriteBatch _spriteBatch;
    private OrthographicCamera _camera;
    private Vector2 viewportResolution = new Vector2(1280, 720);

    private ContextMenu currentSelectedMenu;
    private ContextMenu[] contextMenus;

    private KeyboardListener keyboardListener;

    private void MenuInit()
    {
        keyboardListener = new KeyboardListener();

        menuCenter mainMenu = new menuCenter();
        menuOptions optionsMenu = new menuOptions();
        menuCredits creditsMenu = new menuCredits();
        menuLvlSelect levelSelectMenu = new menuLvlSelect(ScreenManager, Game);

        contextMenus = [
            mainMenu,
            optionsMenu,
            creditsMenu,
            levelSelectMenu
        ];

        // menu linking (so they know who to switch their focus to when called)

        mainMenu.SetMenus(optionsMenu, creditsMenu, levelSelectMenu);
        optionsMenu.switchOnExit = mainMenu;
        creditsMenu.switchOnExit = mainMenu;
        levelSelectMenu.switchOnExit = mainMenu;

        // menu initialization

        foreach (ContextMenu contextMenu in contextMenus)
        {
            contextMenu.Init(Content);
            // keyboardListener.KeyPressed += (_, args) =>
            // {
            //     if (currentSelectedMenu == contextMenu) { contextMenu.OnInputPressed(args.Key); }
            // };
        }

        // 
        SwitchMenu(mainMenu);
        keyboardListener.KeyPressed += screenKeypressFunc;
    }

    private void OnMenuExit()
    {
        SwitchMenu(currentSelectedMenu.switchOnExit);
    }

    private void screenKeypressFunc(object _, KeyboardEventArgs args)
    {
        currentSelectedMenu.OnInputPressed(args.Key);
    }

    private void SwitchMenu(ContextMenu menu)
    {
        if (currentSelectedMenu != null) { 
            currentSelectedMenu.OnFocusExit(); 
            currentSelectedMenu.onExit -= OnMenuExit;
        }
        currentSelectedMenu = menu;
        menu.OnFocusEnter();
        menu.onExit += OnMenuExit;
    }

    public override void Update(GameTime gameTime)
    {
        keyboardListener.Update(gameTime);
        float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

        foreach (ContextMenu contextMenu in contextMenus)
        {
            contextMenu.Update(gameTime);
        }

        _camera.Position = Vector2.Lerp(
        _camera.Position, 
        currentSelectedMenu.cameraPosition - viewportResolution / 2, deltaTime * 5f);
        currentSelectedMenu.FocusedUpdate(gameTime);
    }

    private void MenuUninit()
    {
        keyboardListener.KeyPressed -= screenKeypressFunc;
        foreach (ContextMenu contextMenu in contextMenus)
        {
            contextMenu.Dispose();
        }
    }

    // most of this code down here is just boilerplate for a functional scene
    // this isn't a class because we don't need many scenes.... yet

    public override void Initialize()
    {
        base.Initialize();
        ViewportAdapter viewport = new BoxingViewportAdapter(Game.Window, GraphicsDevice, 
        (int)viewportResolution.X, (int)viewportResolution.Y);
        _camera = new OrthographicCamera(viewport);
        _camera.Position = -viewportResolution / 2;

        MenuInit();
    }

    public override void LoadContent()
    {
        base.LoadContent();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    public override void Draw(GameTime gameTime)
    {
        //GraphicsDevice.Clear(Color.Blue);

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
        Spritekeeper.FreeAll();
        Textkeeper.FreeAll();
        MenuUninit();
    }
}