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

        contextMenus = [
            mainMenu,
            optionsMenu,
        ];

        // menu linking (so they know who to switch their focus to when called)

        mainMenu.optionsMenu = optionsMenu;
        optionsMenu.switchOnExit = mainMenu;

        // menu initialization

        foreach (ContextMenu contextMenu in contextMenus)
        {
            contextMenu.Init(Content);
            keyboardListener.KeyPressed += (_, args) =>
            {
                if (currentSelectedMenu == contextMenu) { contextMenu.OnInputPressed(args.Key); }
            };
        }

        // 

        SwitchMenu(mainMenu);
    }

    private void OnMenuExit()
    {
        SwitchMenu(currentSelectedMenu.switchOnExit);
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

        foreach (ContextMenu contextMenu in contextMenus)
        {
            contextMenu.Update(gameTime);
        }

        _camera.Position = Vector2.Lerp(
        _camera.Position, 
        currentSelectedMenu.cameraPosition - viewportResolution / 2, 0.25f);
        currentSelectedMenu.FocusedUpdate(gameTime);
    }

    private void MenuUninit()
    {
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

        MenuInit();
    }

    public override void LoadContent()
    {
        base.LoadContent();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Blue);

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