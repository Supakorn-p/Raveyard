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
}