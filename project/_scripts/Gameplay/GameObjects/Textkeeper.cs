using System.Diagnostics;
using System.Linq;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Graphics;

namespace Raveyard;

public static class Textkeeper
{
    public static Bag<TextObject> textObjects { get; private set; } = new Bag<TextObject>(16);

    public static TextObject[] getActiveObjs()
    {
        return textObjects.Where(spr => spr.active).ToArray();
    }

    public static void AddToBag(TextObject obj)
    {
        textObjects.Add(obj);
    }

    public static void RemoveFromBag(TextObject obj)
    {
        textObjects.Remove(obj);
    }

    public static void FreeAll()
    {
        foreach (TextObject obj in textObjects.ToArray()) { obj.Free(); }
    }
}