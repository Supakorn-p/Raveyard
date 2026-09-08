using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Raveyard;

public class SpriteObject
{
    private SpriteSheet spriteSheet;
    public AnimatedSprite animatedSprite;

    public string name;
    public Vector2 position;
    public float rotation;
    public float scale;

    public bool active = false;

    public SpriteObject(string _name, Texture2DAtlas texture2Datlas, Vector2 _position)
    {
        //texture = texture2D;
        position = _position;
        name = _name;

        createAnimatedSprite(texture2Datlas);
        Spritekeeper.AddToBag(this);
    }

    private void createAnimatedSprite(Texture2DAtlas atlas)
    {
        spriteSheet = new SpriteSheet($"spritesheet/{name}", atlas);
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