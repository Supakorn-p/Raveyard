using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Raveyard;

public class SpriteObject : DrawableObject
{
    private SpriteSheet spriteSheet;
    public AnimatedSprite animatedSprite;

    public Vector2 position;
    public Vector2 position_offset;
    public Vector2 anchor = new Vector2(0.5f, 0.5f);
    public float rotation;
    public Vector2 scale = new Vector2(1,1);
    public int layer = 0;
    public float alpha = 1.0f;

    public readonly Tweener tweener = new Tweener();

    public SpriteObject(string _name, Texture2D texture2D, Vector2 region, Vector2 _position, int startingFrame = 0)
    {
        //texture = texture2D;
        position = _position;
        name = _name;

        Texture2DAtlas texture2Datlas = Texture2DAtlas.Create($"atl/{name}", texture2D, (int) region.X, (int) region.Y);
        createAnimatedSprite(texture2Datlas, startingFrame);
        Drawablekeeper.AddToBag(this);
    }

    private void createAnimatedSprite(Texture2DAtlas atlas, int startingFrame)
    {
        spriteSheet = new SpriteSheet($"spritesheet/{name}", atlas);
        spriteSheet.DefineAnimation("default", builder =>
        {
           builder.IsLooping(true);
           builder.AddFrame(startingFrame, TimeSpan.FromDays(1)); 
        });
        animatedSprite = new AnimatedSprite(spriteSheet, "default");
    }

    public void LoadAnim(string animName, int[] frames, TimeSpan duration, bool looping = false)
    {
        spriteSheet.DefineAnimation(animName, builder =>
        {
            builder.IsLooping(looping);

            for (int i = 0; i < frames.Length; i++)
            {
                builder.AddFrame(frames[i], duration);
            }
        });
    }

    private const int maxLayers = 20; // this goes both ways (6 = -3 to 3)
    private float LayerToDepth()
    {
        float trueLayer = (layer + maxLayers/2f) / maxLayers;
        return Math.Clamp(1.0f - trueLayer, 0.0f, 1.0f);
    }

    public void SetSpriteValues()
    {
        animatedSprite.OriginNormalized = anchor;
        animatedSprite.Depth = LayerToDepth();
        animatedSprite.Alpha = alpha;
    }

    public override void Update(GameTime gameTime)
    {
        animatedSprite.Update(gameTime);
        tweener.Update(gameTime.GetElapsedSeconds());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float _rotation = rotation/180f * MathF.PI;
        SetSpriteValues();
        spriteBatch.Draw(animatedSprite, position, _rotation, scale);
    }
}