using Microsoft.Xna.Framework.Input;

namespace Raveyard;

public static class SETTINGS
{
    public const Keys keybind_back = Keys.Escape;

    // it's impossible to play input feedback ahead of time
    // so to prevent it from sounding off, it assumes you did it perfectly
    public static bool AUDIO_FEEDBACK_PURE_MODE = true; 
    public static int AUDIO_JUDGEMENT_OFFSET_MS = 0; // only works if above option is false
}

