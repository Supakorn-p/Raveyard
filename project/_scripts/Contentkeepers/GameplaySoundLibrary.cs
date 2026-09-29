using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Raveyard;

public class GameplaySoundLibrary
{
    private static Dictionary<string, SoundEffect> sfxs = new Dictionary<string, SoundEffect>();
    private ContentManager content;

    public GameplaySoundLibrary(ContentManager _content)
    {
        content = _content;
    }

    public void LoadSound(string asset_name)
    {
        sfxs.Add(asset_name, content.Load<SoundEffect>(asset_name));
    }

    public static void PlaySound(string asset_name, float volume = 1.0f, float pan = 0.0f, float pitch = 0.0f)
    {
        if (!sfxs.ContainsKey(asset_name)) { throw new System.Exception($"[SNDLIB] {asset_name} isn't loaded!"); }
        sfxs[asset_name].Play(volume, pitch, pan);
    }

    public void UnloadAll()
    {
        foreach (string asset_name in sfxs.Keys) { content.UnloadAsset(asset_name); }
        sfxs.Clear();
    }
}