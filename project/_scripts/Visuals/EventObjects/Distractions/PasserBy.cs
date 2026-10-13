using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Raveyard._scripts.Visuals.EventObjects.Distractions
{
    internal class PasserBy : EventObject
    {
        public SpriteObject ghost;
        public Vector2 position = new Vector2(-1100, 100);

        private static Texture2D ghostTexture;

        public override void Init(ContentManager Content)
        {
            base.Init(Content);
            eventName = "dst_passerby";

            Texture2D ghostTexture = Content.Load<Texture2D>("ghost_sprsheet");

            ghost = new SpriteObject("ghost", ghostTexture, new Vector2(740, 576), position);
            ghost.layer += 5;
            ghost.active = true;
        }

        private void Move()
        {
            ghost.position.X += 5;

            //if (isMoving == true)
            //{
            //    if (goUp == false)
            //    {
            //        MoveDown();
            //    }
            //    else
            //    {
            //        MoveUp();
            //    }
            //    isMoving = false;
            //}
        }

        bool isMoving = false;
        bool goUp = false;

        private void ChangeValue(bool newVal)
        {
            goUp = newVal;
            isMoving = true;
        }
        private void MoveUp()
        {
            ghost.tweener.TweenTo(target: ghost, expression: position => position.position, toValue: new Vector2(ghost.position.X, 100f), duration: 1f)
                .Easing(EasingFunctions.SineInOut)
                .OnEnd(tween => ChangeValue(false));
        }

        private void MoveDown()
        {
            ghost.tweener.TweenTo(target: ghost, expression: position => position.position, toValue: new Vector2(ghost.position.X, 130f), duration: 1f)
                .Easing(EasingFunctions.SineInOut)
                .OnEnd(tween => ChangeValue(true));

        }


        public override void OnEvent(string[] eventParams)
        {
            base.OnEvent(eventParams);
            isMoving = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isMoving == true)
            {
                Move();
                if (ghost.position.X > 1100)
                {
                    ghost.Free();
                }
            }
        }
    }
}
