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
using Microsoft.Xna.Framework.Input;

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

    private void MenuMouseHovering(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        float mouseX = Mouse.GetState().Position.X;
        float mouseY = Mouse.GetState().Position.Y;

        Vector2 mouseOffset = new Vector2(mouseX, mouseY) - (viewportResolution / 2);

        Vector2 newCameraPos = (currentSelectedMenu.cameraPosition - viewportResolution / 2) + (mouseOffset * 0.15f);
        //Vector2 viewportCamera = new Vector2(mouseX - viewportHalved.X, mouseY - viewportHalved.Y);

        _camera.Position = Vector2.Lerp(_camera.Position, newCameraPos, delta * 5f);
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
        //_camera.Position, 
        //currentSelectedMenu.cameraPosition, deltaTime * 5f);

        //_camera.Position = currentSelectedMenu.cameraPosition - viewportResolution / 2;
        currentSelectedMenu.FocusedUpdate(gameTime);

        MenuMouseHovering(gameTime);
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
            Vector2 _scale = new Vector2(1.25f, 1.25f);

            spriteObj.SetSpriteValues();

            _spriteBatch.Draw(spriteObj.animatedSprite, spriteObj.position, _rotation, _scale);
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