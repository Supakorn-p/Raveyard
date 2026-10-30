using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Linq;

namespace Raveyard;

public class scMenu : GameScreen
{
    public scMenu(Game game) : base(game) {}

    private SpriteBatch _spriteBatch;
    private OrthographicCamera _camera;
    private Vector2 viewportResolution = new Vector2(1280, 720);

    Song bgm;

    private ContextMenu currentSelectedMenu;
    private ContextMenu[] contextMenus;

    private KeyboardListener keyboardListener;

    public menuCenter mainMenu;
    public menuOptions optionsMenu;
    public menuCredits creditsMenu;
    public menuLvlSelect levelSelectMenu;

    private void MenuInit()
    {
        keyboardListener = new KeyboardListener();

        mainMenu = new menuCenter();
        optionsMenu = new menuOptions();
        creditsMenu = new menuCredits();
        levelSelectMenu = new menuLvlSelect(ScreenManager, Game);

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

    private void MenuMouseHovering(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        float mouseX = Mouse.GetState().Position.X;
        float mouseY = Mouse.GetState().Position.Y;

        Vector2 mouseOffset = new Vector2(mouseX, mouseY) - (viewportResolution / 2);

        Vector2 newCameraPos = (currentSelectedMenu.cameraPosition - viewportResolution / 2) + (mouseOffset * 0.025f);
        //Vector2 viewportCamera = new Vector2(mouseX - viewportHalved.X, mouseY - viewportHalved.Y);

        _camera.Position = Vector2.Lerp(_camera.Position, newCameraPos, delta * 5f);
    }

    private void ParallaxObject(GameTime gameTime, SpriteObject _sprite, ContextMenu _menu, float _amount, Vector2 _offset = default)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        float mouseX = Mouse.GetState().Position.X;
        float mouseY = Mouse.GetState().Position.Y;

        Vector2 mouseOffset = new Vector2(mouseX, mouseY) - (viewportResolution / 2);

        Vector2 newObjectPos = (_menu.cameraPosition + _offset) + (mouseOffset * _amount);

        _sprite.position = Vector2.Lerp(_sprite.position, newObjectPos, delta * 5f);
    }

    private void ParallaxAllObjects(GameTime gameTime)
    {
        ParallaxObject(gameTime, mainMenu.titleText, mainMenu, 0.02f);
    }

    public override void Update(GameTime gameTime)
    {
        keyboardListener.Update(gameTime);
        float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

        foreach (ContextMenu contextMenu in contextMenus)
        {
            contextMenu.Update(gameTime);
        }

        //_camera.Position = Vector2.Lerp(
       // _camera.Position, 
        //currentSelectedMenu.cameraPosition - viewportResolution / 2, deltaTime * 5f);
        currentSelectedMenu.FocusedUpdate(gameTime);
        MenuMouseHovering(gameTime);

        ParallaxAllObjects(gameTime);

        foreach (DrawableObject drawObj in Drawablekeeper.getActiveObjs())
        {
            drawObj.Update(gameTime);
        }
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

        foreach (DrawableObject drawObj in Drawablekeeper.getActiveObjs())
        {
            drawObj.Draw(_spriteBatch);
        }

        _spriteBatch.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        Drawablekeeper.FreeAll();
        MenuUninit();
    }
}