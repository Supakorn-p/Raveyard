using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Raveyard;

public static class DebugTool
{
    public static bool isDebugOn = true;
    
    public static bool debugOption_autoplay {get; private set;} = false;

    public static void OnKeyPressed(KeyboardStateExtended keyboardState)
    {
        if (keyboardState.WasKeyPressed(Keys.D0))
        {
            debugOption_autoplay = !debugOption_autoplay && isDebugOn;
            Debug.WriteLine($"autoplay: {debugOption_autoplay}");
        }
    }
}