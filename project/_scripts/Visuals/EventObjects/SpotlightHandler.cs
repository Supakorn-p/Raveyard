using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace Raveyard;

public class SpotlightHandler : EventObject
{
    private SpriteObject spotlightObj;

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "spotlight";
        spotlightObj.LoadAnim("type0", [0], TimeSpan.FromHours(1));
        spotlightObj.LoadAnim("type1", [1], TimeSpan.FromHours(1));
        spotlightObj.LoadAnim("type2", [2], TimeSpan.FromHours(1));
        spotlightObj.LoadAnim("type3", [3], TimeSpan.FromHours(1));
    }

    public void InjectSpotlightObj(SpriteObject sprite)
    {
        spotlightObj = sprite;
    }

    // to convert real duration to beatSec
    double beatSeconds = 1;

    public void InjectBPM(double bpm) { beatSeconds = 60 / bpm; }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (spotlightObj == null) { return; }
        spotlightObj.tweener.Update(gameTime.GetElapsedSeconds());
    }

    public override void OnEvent(string[] eventParams)
    {
        base.OnEvent(eventParams);
        if (spotlightObj == null) { return; }
        // spotlight:0|ACTION
        if (eventParams.Length < 2) { return; }
        string ACTION = eventParams[0];

        if (ACTION == "FADE")
        {
            // spotlight:0|ACTION|ALPHA|DURATION
            if (eventParams.Length < 3) { return; }
            float.TryParse(eventParams[1], out float ALPHA);
            float.TryParse(eventParams[2], out float DURATION);
            spotlightObj.tweener.CancelAll();
            spotlightObj.tweener.TweenTo(spotlightObj, player => player.alpha, ALPHA, DURATION * (float) beatSeconds)
            .Easing(EasingFunctions.Linear);
        }
        if (ACTION == "TYPE")
        {
            // spotlight:0|TYPE|[number]
            int.TryParse(eventParams[1], out int TYPE);
            spotlightObj.animatedSprite.SetAnimation($"type{TYPE}");
        }
    }
}