using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;

namespace Raveyard;

public class DialogTextBubble : EventObject
{
    private TextObject textObj;
    private SpriteObject dialogBoxSpr;

    private Vector2 dialogBoxPos = new Vector2(0, -200);
    private Vector2 dialogBoxBouncePos = new Vector2(0, -10);

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "dialog_bubble";

        dialogBoxSpr = new SpriteObject("radio_box", Content.Load<Texture2D>("dialogbox_placeholder"),
        new Vector2(900, 300), dialogBoxPos);
        dialogBoxSpr.scale = new Vector2(1.25f, 0.5f);
        dialogBoxSpr.layer = 5;

        SpriteFont font = Content.Load<SpriteFont>("anton_sc");
        textObj = new TextObject(font, new StringBuilder(""));

        textObj.position = dialogBoxPos;
        textObj.scale = new Vector2(0.5f, 0.5f);
        textObj.layer = 11;
        textObj.color = Color.Black;
        textObj.active = true;

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void OnEvent(string[] eventParams)
    {
        base.OnEvent(eventParams);

        // bounce
        dialogBoxSpr.tweener.CancelAll();
        dialogBoxSpr.position = dialogBoxBouncePos + dialogBoxPos;
        dialogBoxSpr.tweener.TweenTo(target: dialogBoxSpr, expression: player => player.position, toValue: dialogBoxPos, 
            duration: 0.3f)
            .Easing(EasingFunctions.CubicOut);

        textObj.tweener.CancelAll();
        textObj.position = dialogBoxBouncePos + dialogBoxPos;
        textObj.tweener.TweenTo(target: textObj, expression: player => player.position, toValue: dialogBoxPos, 
            duration: 0.3f)
            .Easing(EasingFunctions.CubicOut);

        // set text
        textObj.text = new StringBuilder(eventParams[0]);
        dialogBoxSpr.active = eventParams[0] != "";
    }
}