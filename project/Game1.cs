using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Raveyard;

/* 
!!

THIS IS FOR HANDLING THE MAIN GAME WINDOW
YOU MAY BE LOOKING FOR ONE OF THE SCENES INSTEAD

!!
*/

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private void playBeep(EventParams parameters)
    {
        beep?.Play();
    }

    private Timeline timeline = new Timeline();
    protected override void Initialize()
    {
        timeline.addEvent("event", 1);
        timeline.addEvent("event2", 2);
        timeline.addEvent("event2", 3);
        timeline.addEvent("event", 2.5);
        timeline.addEvent("event", 4);
        timeline.addEvent("event", 4 + 1/6.0);
        timeline.addEvent("event", 4 + 2/6.0);
        timeline.addEvent("event", 4 + 3/6.0);

        timeline.subscribeToEvent("event", playBeep);
        timeline.subscribeToEvent("event2", playBeep);
        
        base.Initialize();
    }

    Song bgmTest;
    SoundEffect beep;
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        bgmTest = Song.FromUri("_charts/prototype.ogg", new Uri("_charts/prototype.ogg", UriKind.Relative));
        MediaPlayer.Play(bgmTest);
        beep = Content.Load<SoundEffect>("beep");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        timeline.Update(gameTime.TotalGameTime.TotalMilliseconds / 1000.0);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        base.Draw(gameTime);
    }
}
