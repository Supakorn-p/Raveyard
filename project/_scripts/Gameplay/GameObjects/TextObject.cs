using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace Raveyard;

public class TextObject : DrawableObject
{
    public SpriteFont font;
    public StringBuilder text;

    public Vector2 position;
    public float rotation;
    public Vector2 scale = new Vector2(1,1);
    public Color color = Color.White;
    public float alpha = 1f;
    public Vector2 anchor = new Vector2(0.5f, 0.5f);

    public readonly Tweener tweener = new Tweener();

    public TextObject(SpriteFont _font, StringBuilder _string)
    {
        font = _font;
        text = _string;
        Drawablekeeper.AddToBag(this);
    }

    public Vector2 GetRawOrigin()
    {
        return anchor * font.MeasureString(text);
    }

    public override void Update(GameTime gameTime)
    {
        tweener.Update(gameTime.GetElapsedSeconds());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float _rotation = rotation/180f * MathF.PI;

        spriteBatch.DrawString(font, text, position, 
        new Color(color, alpha), _rotation, GetRawOrigin(), scale,
        SpriteEffects.None, LayerToDepth());
    }
}