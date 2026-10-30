using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;

namespace Raveyard;

public class DialogTextBubble : EventObject
{
    private TextObject textObj;
    private NinepatchSpriteObject dialogBoxSpr;

    private SpriteObject radioSpr;

    private const string sfx_name = "snd_dialog_tutorial";

    private Vector2 dialogBoxPos = new Vector2(0, 200);
    private Vector2 radioPos = new Vector2(-225, 0);
    private Vector2 radioBouncePos = new Vector2(0, -20);

    private Vector2 dialogBoxStartSize = new Vector2(32, 32);
    private Vector2 dialogBoxEndSize = new Vector2(900, 50);

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "dialog_bubble";
        new GameplaySoundLibrary(Content).LoadSound(sfx_name);

        dialogBoxSpr = new NinepatchSpriteObject("radio_box", Content.Load<Texture2D>("ui_ninepatch_autumn"),
        dialogBoxStartSize, dialogBoxPos);
        dialogBoxSpr.layer = 10;

        SpriteFont font = Content.Load<SpriteFont>("anton_sc");
        textObj = new TextObject(font, new StringBuilder(""));

        textObj.position = dialogBoxPos;
        textObj.scale = new Vector2(0.5f, 0.5f);
        textObj.layer = 11;
        textObj.color = Color.Black;
        textObj.active = true;

        radioSpr = new SpriteObject("radio", Content.Load<Texture2D>("tutorial_radio"),
                                    new Vector2(300, 300), radioPos);
        radioSpr.layer = 0;
        radioSpr.active = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    private void RadioBounce()
    {
        radioSpr.tweener.CancelAll();
        radioSpr.tweener.TweenTo(target: radioSpr, expression: player => player.position, toValue: radioBouncePos + radioPos, duration: 0.1f)
              .Easing(EasingFunctions.CubicOut)
              .OnEnd(tween => radioSpr.tweener.TweenTo(target: radioSpr, expression: player => player.position, toValue: radioPos, duration: 1f)
              .Easing(EasingFunctions.BounceOut));
    }

    public override void OnEvent(string[] eventParams)
    {
        base.OnEvent(eventParams);
        string text = eventParams[0];

        // early returns 
        if (text == "") 
        { 
            dialogBoxSpr.tweener.CancelAll();
            dialogBoxSpr.tweener.TweenTo(target: dialogBoxSpr, expression: player => player.alpha, toValue: 0f, 
            duration: 0.1f)
            .Easing(EasingFunctions.Linear).OnEnd(tween => dialogBoxSpr.active = true);
            textObj.active = false;
            return;
        }
        if (text == "!INVERT_POS") { dialogBoxPos = -dialogBoxPos; return; }

        // vvv real animations below
        GameplaySoundLibrary.PlaySound(sfx_name);
        dialogBoxSpr.active = true;
        textObj.active = false;
        textObj.position = dialogBoxPos;

        RadioBounce();

        // appear
        dialogBoxSpr.tweener.CancelAll();
        dialogBoxSpr.position = radioPos;
        dialogBoxSpr.tweener.TweenTo(target: dialogBoxSpr, expression: player => player.position, toValue: dialogBoxPos, 
            duration: 0.1f)
            .Easing(EasingFunctions.Linear);
        
        dialogBoxSpr.box_size = dialogBoxStartSize;
        dialogBoxSpr.tweener.TweenTo(target: dialogBoxSpr, expression: player => player.box_size, toValue: dialogBoxEndSize, 
            duration: 0.1f, delay: 0.1f)
            .Easing(EasingFunctions.BackOut);

        dialogBoxSpr.alpha = 0.75f;
        dialogBoxSpr.tweener.TweenTo(target: dialogBoxSpr, expression: player => player.alpha, toValue: 1f, 
            duration: 0.1f, delay: 0.1f)
            .Easing(EasingFunctions.Linear).OnEnd(tween => textObj.active = true);

        // set text
        textObj.text = new StringBuilder(eventParams[0]);
    }
}