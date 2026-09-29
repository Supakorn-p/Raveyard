using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raveyard;

public class TextObject
{
    public SpriteFont font;
    public StringBuilder text;

    public Vector2 position;
    public float rotation;
    public Vector2 scale = new Vector2(1,1);
    public int layer = 0;
    public Color color = Color.White;
    public float alpha = 1.0f;
    public Vector2 anchor = new Vector2(0.5f, 0.5f);

    public bool active = false;

    public TextObject(SpriteFont _font, StringBuilder _string)
    {
        font = _font;
        text = _string;
        Textkeeper.AddToBag(this);
    }

    public void Free()
    {
        Textkeeper.RemoveFromBag(this);
    }

    public Vector2 GetRawOrigin()
    {
        return anchor * font.MeasureString(text);
    }

    private const int maxLayers = 20; // this goes both ways (6 = -3 to 3)
    public float LayerToDepth()
    {
        float trueLayer = (layer + maxLayers/2f) / maxLayers;
        return Math.Clamp(1.0f - trueLayer, 0.0f, 1.0f);
    }
}