using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Raveyard;

public class SpriteObject
{
    private SpriteSheet spriteSheet;
    public AnimatedSprite animatedSprite;

    public string name;
    public Vector2 position;
    public Vector2 anchor = new Vector2(0.5f, 0.5f);
    public float rotation;
    public float scale;

    public bool active = false;

    public SpriteObject(string _name, Texture2D texture2D, Rectangle region, Vector2 _position)
    {
        //texture = texture2D;
        position = _position;
        name = _name;

        Texture2DAtlas texture2Datlas = new Texture2DAtlas(texture2D);
        texture2Datlas.CreateRegion(region, $"atl/{name}");
        createAnimatedSprite(texture2Datlas);
        Spritekeeper.AddToBag(this);
    }

    private void createAnimatedSprite(Texture2DAtlas atlas)
    {
        spriteSheet = new SpriteSheet($"spritesheet/{name}", atlas);
        spriteSheet.DefineAnimation("default", builder =>
        {
           builder.IsLooping(true);
           builder.AddFrame(0, TimeSpan.FromTicks(1)); 
        });
        animatedSprite = new AnimatedSprite(spriteSheet, "default");
    }

    public void LoadAnim(string animName, int numberOfFrames, TimeSpan duration, bool looping = false)
    {
        spriteSheet.DefineAnimation(animName, builder =>
        {
            builder.IsLooping(looping);

            for (int i = 0; i < numberOfFrames; i++)
            {
                builder.AddFrame(i, duration);
            }
        });
    }

    public void Free()
    {
        Spritekeeper.RemoveFromBag(this);
    }
}