using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;

namespace Raveyard;

/* 
!!

THIS IS FOR HANDLING THE MAIN GAME WINDOW
YOU MAY BE LOOKING FOR ONE OF THE SCENES INSTEAD

Unrelated Text

!!
*/

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ScreenManager _screenManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
    }

    protected override void Initialize()
    {
        base.Initialize();
        _screenManager.ShowScreen(new scGameplay(this, "prototype"));
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // poll input
        KeyboardExtended.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        base.Draw(gameTime);
    }
}
