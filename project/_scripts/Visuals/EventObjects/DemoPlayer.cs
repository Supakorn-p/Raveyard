using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace Raveyard;

public class DemoPlayer : EventObject
{
    private TextObject textObj;
    private bool demoToggle = false;

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "demo_toggle";

        SpriteFont font = Content.Load<SpriteFont>("anton_sc");
        textObj = new TextObject(font, new StringBuilder("EXAMPLE"));

        textObj.anchor = new Vector2(0f, 1f);
        textObj.position = new Vector2(-1280/2, 720/2) + new Vector2(20, -20);
        textObj.layer = 32;
        textObj.color = Color.DarkGray;

        DebugTool.ForceSet(false);
    }

    private double visualTimer = 0;
    private GameTime _gameTime;
    public override void Update(GameTime gameTime)
    {
        _gameTime = gameTime;
        base.Update(gameTime);
        textObj.active = DebugTool.debugOption_autoplay;
        textObj.alpha = (float) Math.Abs(Math.Sin(
            (gameTime.TotalGameTime.TotalSeconds - visualTimer)
            / (Math.PI/4)));
    }

    public override void OnEvent(string[] eventParams)
    {
        base.OnEvent(eventParams);

        demoToggle = eventParams[0] == "true";
        DebugTool.ForceSet(demoToggle);
        visualTimer = _gameTime.TotalGameTime.TotalSeconds;
    }
}