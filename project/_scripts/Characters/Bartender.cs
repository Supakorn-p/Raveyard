using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Tweening;

namespace Raveyard._scripts.Characters
{
    public class Bartender
    {
        public SpriteObject bartender;
        public Vector2 position;

        private enum States
        {
            takingOrder,
            makingOrder,
            success,
            fail
        }

        private States State = States.takingOrder;

        public Bartender()
        {
            position = new Vector2(125, 170);
        }

        public void BartenderInitialize()
        {
            bartender.anchor = new Vector2(0.5f, 1f);
            bartender.LoadAnim("bar_idle", [0, 0, 1], TimeSpan.FromMilliseconds(240), true); // Idle

            TakingOrder();
        }


        private void TakingOrder() // Basically Idle
        {
            bartender.animatedSprite.SetAnimation("bar_idle");
        }

        public void OnInput()
        {
            bartender.tweener.CancelAll();
            bartender.scale = new Vector2(1.0f, 0.9f);
            bartender.tweener.TweenTo(bartender, player => player.scale, new Vector2(1f, 1f), 0.2f)
            .Easing(EasingFunctions.CubicOut);
        }

        private void MakingOrder()
        {

        }

    }
}
