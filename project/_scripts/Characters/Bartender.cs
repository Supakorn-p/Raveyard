using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
            position = new Vector2(125, -78);
        }

        public void BartenderInitialize()
        {
            // Load Animations
            bartender.LoadAnim("bar_idle", [0, 0, 1], TimeSpan.FromMilliseconds(240), true); // Idle

            TakingOrder();
        }


        private void TakingOrder() // Basically Idle
        {
            bartender.animatedSprite.SetAnimation("bar_idle");
        }

        private void MakingOrder()
        {

        }

    }
}
