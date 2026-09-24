using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace Raveyard;

public class TextureCollection
{
    private static Dictionary<string, Texture2D> assets = new Dictionary<string, Texture2D>();
    private ContentManager content;

    public TextureCollection(ContentManager _content)
    {
        content = _content;
    }

    public void LoadTexture(string asset_name)
    {
        assets.Add(asset_name, content.Load<Texture2D>(asset_name));
    }

    public Texture2D GetTexture(string asset_name)
    {
        return assets[asset_name];
    }

    public void UnloadAll()
    {
        foreach (string asset_name in assets.Keys) { content.UnloadAsset(asset_name); }
        assets.Clear();
    }
}