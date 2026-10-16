using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raveyard;

public abstract class DrawableObject
{
    public string name = "";
    public bool active = false;

    public virtual void Update(GameTime gameTime) {}
    public abstract void Draw(SpriteBatch spriteBatch);
    public virtual void Free()
    {
        Drawablekeeper.RemoveFromBag(this);
    }

    public int layer;
    private const int maxLayers = 64; // this goes both ways (eg. 6 = -3 to 3)
    public float LayerToDepth()
    {
        float trueLayer = (layer + maxLayers/2f) / maxLayers;
        return Math.Clamp(1.0f - trueLayer, 0.0f, 1.0f);
    }
}