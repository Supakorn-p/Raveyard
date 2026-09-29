using System.Diagnostics;
using System.Linq;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Graphics;

namespace Raveyard;

public static class Spritekeeper
{
    public static Bag<SpriteObject> spriteObjects { get; private set; } = new Bag<SpriteObject>(16);

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

    public static void FreeAll()
    {
        foreach (SpriteObject obj in spriteObjects.ToArray()) { obj.Free(); }
    }

    private static void debug_PrintAll()
    {
        Debug.WriteLine("===");
        for (int i = 0; i < spriteObjects.Count; i++)
        {
            SpriteObject obj = spriteObjects[i];
            string objName = obj != null ? obj.name : "----------";
            Debug.WriteLine($"{i}: {objName}");
        }
    }
}