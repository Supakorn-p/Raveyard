using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Raveyard;

public class NinepatchSpriteObject : DrawableObject
{
    private Texture2D ninePatchTexture;

    public Vector2 position;
    public Vector2 position_offset;
    public Vector2 anchor = new Vector2(0.5f, 0.5f);
    //public Vector2 scale = new Vector2(1.0f, 1.0f);
    public float alpha = 1f;

    public Vector2 box_size = new Vector2(32, 32);
    public float box_border_size = 64;

    public readonly Tweener tweener = new Tweener();

    public NinepatchSpriteObject(string _name, Texture2D texture2D, Vector2 _box_size, Vector2 _position)
    {
        //texture = texture2D;
        position = _position;
        name = _name;
        box_size = _box_size;
        ninePatchTexture = texture2D;
        Drawablekeeper.AddToBag(this);
    }

    public override void Update(GameTime gameTime)
    {
        tweener.Update(gameTime.GetElapsedSeconds());
    }

    private Rectangle getTextureRegionRect(int number)
    {
        Vector2 regionSize = new Vector2(ninePatchTexture.Width / 3f, ninePatchTexture.Height / 3f);
        Vector2 regionPos = new Vector2(number % 3,
                                        number / 3);
        return new Rectangle((int) regionPos.X * (int) regionSize.X, (int) regionPos.Y * (int) regionSize.Y, 
                             (int) regionSize.X, (int) regionSize.Y);
    }

    // i got tired of boilerplate
    private void DrawRegion(SpriteBatch spriteBatch, Rectangle rectDrawRegion, Rectangle rectTextureRegion)
    {
        spriteBatch.Draw(ninePatchTexture, rectDrawRegion, rectTextureRegion, new Color(1f, 1f, 1f, alpha),
        0.0f, Vector2.Zero, SpriteEffects.None, LayerToDepth());
    }

    // it looks ugly but im no mathmatician
    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 true_position = position - box_size * anchor;

        // draw center
        Rectangle centerRect = new Rectangle((int) true_position.X, (int) true_position.Y,
                                             (int) box_size.X, (int) box_size.Y);
        DrawRegion(spriteBatch, centerRect, getTextureRegionRect(4));

        // draw edges
        Rectangle edgeHorizontalRect1 = new Rectangle((int) true_position.X, (int) (true_position.Y - box_border_size), 
                                                      (int) box_size.X, (int) box_border_size);
        Rectangle edgeHorizontalRect2 = new Rectangle((int) true_position.X, (int) (true_position.Y + box_size.Y), 
                                                      (int) box_size.X, (int) box_border_size);

        Rectangle edgeVerticalRect1 = new Rectangle((int) (true_position.X - box_border_size), (int) true_position.Y, 
                                                      (int) box_border_size, (int) box_size.Y);
        Rectangle edgeVerticalRect2 = new Rectangle((int) (true_position.X + box_size.X), (int) true_position.Y, 
                                                      (int) box_border_size, (int) box_size.Y);

        DrawRegion(spriteBatch, edgeHorizontalRect1, getTextureRegionRect(1));
        DrawRegion(spriteBatch, edgeHorizontalRect2, getTextureRegionRect(7));
        DrawRegion(spriteBatch, edgeVerticalRect1, getTextureRegionRect(3));
        DrawRegion(spriteBatch, edgeVerticalRect2, getTextureRegionRect(5));

        // draw corners
        Rectangle cornerRectTL = new Rectangle((int) (true_position.X - box_border_size), (int) (true_position.Y - box_border_size),
                                               (int) box_border_size, (int) box_border_size);
        Rectangle cornerRectTR = new Rectangle((int) (true_position.X + box_size.X), (int) (true_position.Y - box_border_size),
                                               (int) box_border_size, (int) box_border_size);
        Rectangle cornerRectBL = new Rectangle((int) (true_position.X - box_border_size), (int) (true_position.Y + box_size.Y),
                                               (int) box_border_size, (int) box_border_size);
        Rectangle cornerRectBR = new Rectangle((int) (true_position.X + box_size.X), (int) (true_position.Y + box_size.Y),
                                               (int) box_border_size, (int) box_border_size);

        DrawRegion(spriteBatch, cornerRectTL, getTextureRegionRect(0));
        DrawRegion(spriteBatch, cornerRectTR, getTextureRegionRect(2));
        DrawRegion(spriteBatch, cornerRectBL, getTextureRegionRect(6));
        DrawRegion(spriteBatch, cornerRectBR, getTextureRegionRect(8));
    }
}