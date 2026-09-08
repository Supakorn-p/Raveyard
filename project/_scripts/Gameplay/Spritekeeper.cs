using System.Linq;
using MonoGame.Extended.Collections;

namespace Raveyard;

public static class Spritekeeper
{
    public static Bag<SpriteObject> spriteObjects { get; private set; } = new Bag<SpriteObject>(4);

    public static SpriteObject[] getActiveObjs()
    {
        return spriteObjects.Where(spr => spr.active).ToArray();
    }

    public static void AddToBag(SpriteObject obj)
    {
        spriteObjects.Add(obj);
    }

    public static void RemoveFromBag(SpriteObject obj)
    {
        spriteObjects.Remove(obj);
    }
}