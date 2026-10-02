using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Raveyard;

public class DialogTextBubble : EventObject
{
    private TextObject textObj;

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "dialog_bubble";

        SpriteFont font = Content.Load<SpriteFont>("anton_sc");
        textObj = new TextObject(font, new StringBuilder(""));
        textObj.active = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void OnEvent(string[] eventParams)
    {
        base.OnEvent(eventParams);

        textObj.text = new StringBuilder(eventParams[0]);
    }
}