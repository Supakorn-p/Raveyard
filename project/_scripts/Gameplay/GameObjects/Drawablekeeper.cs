using System.Diagnostics;
using System.Linq;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Graphics;

namespace Raveyard;

public static class Drawablekeeper
{
    public static Bag<DrawableObject> objects { get; private set; } = new Bag<DrawableObject>(64);

    public static DrawableObject[] getActiveObjs()
    {
        return objects.Where(spr => spr.active).ToArray();
    }

    public static void AddToBag(DrawableObject obj)
    {
        objects.Add(obj);
    }

    public static void RemoveFromBag(DrawableObject obj)
    {
        objects.Remove(obj);
    }

    public static void FreeAll()
    {
        foreach (DrawableObject obj in objects.ToArray()) { obj.Free(); }
    }

    private static void debug_PrintAll()
    {
        Debug.WriteLine("===");
        for (int i = 0; i < objects.Count; i++)
        {
            DrawableObject obj = objects[i];
            string objName = obj != null ? obj.name : "----------";
            Debug.WriteLine($"{i}: {objName}");
        }
    }
}