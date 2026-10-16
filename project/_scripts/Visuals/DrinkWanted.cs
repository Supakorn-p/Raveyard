using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;

namespace Raveyard._scripts.Visuals
{
    public class DrinkWanted
    {
        public SpriteObject orderedDrink;
        public Vector2 position = new Vector2(-450, -225);
        public Vector2 scale = Vector2.Zero;
        public float rotation = -200f;

        private static Texture2D drinkTexture;

        public DrinkWanted()
        {

        }

        public static void InitializeWantedDrink(Texture2D _drinkTexture)
        {
            drinkTexture = _drinkTexture;
        }

        public void CreateDrinkSprite()
        {
            orderedDrink = new SpriteObject("order_box", drinkTexture, new Vector2(370, 370), new Vector2(-350, -250), 1);
            orderedDrink.scale = scale;
            orderedDrink.rotation = rotation;
            orderedDrink.active = true;
        }

        public void StartOrder()
        {
            /* I'm going to be honest I have no idea how I could actually optimize this, altho I might have sommething later
             down the line, but I'll hardcode it for now */

            orderedDrink.tweener.TweenTo(target: orderedDrink, expression: player => player.scale, toValue: new Vector2(0.75f, 0.75f), duration: 0.3f)
            .Easing(EasingFunctions.CubicInOut);

            orderedDrink.tweener.TweenTo(target: orderedDrink, expression: player => player.rotation, toValue: 0, duration: 0.6f)
            .Easing(EasingFunctions.CubicOut);

            orderedDrink.tweener.TweenTo(target: orderedDrink, expression: player => player.position, toValue: position, duration: 0.8f)
           .Easing(EasingFunctions.CubicOut);
        }

        public void SuccessInput()
        {
            orderedDrink.tweener.TweenTo(target: orderedDrink, expression: player => player.position, toValue: new Vector2(-450, -235), duration: 0.1f)
           .Easing(EasingFunctions.CubicOut)
           .OnEnd(tween => orderedDrink.tweener.TweenTo(target: orderedDrink, expression: player => player.position, toValue: position, duration: 0.1f)
           .Easing(EasingFunctions.CubicIn));

        }

        public void EndOrder()
        {
            /* I'm going to be honest I have no idea how I could actually optimize this, altho I might have sommething later
             down the line, but I'll hardcode it for now */

            orderedDrink.tweener.TweenTo(target: orderedDrink, expression: player => player.scale, toValue: new Vector2(0, 0), duration: 0.3f)
            .Easing(EasingFunctions.CubicInOut)
            .OnEnd(tween => DestroyWantedDrink());
        }

        public void DestroyWantedDrink()
        {
            orderedDrink.Free();
        }
    }
}
