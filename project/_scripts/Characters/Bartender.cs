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

        public Bartender()
        {
            position = new Vector2(125,-78);
        }

        public void BartenderInitialize()
        {
            // Load Animations
            bartender.LoadAnim("bar_idle", 6, TimeSpan.FromTicks(1), new Vector2(0, 1), true); // Idle

            TakingOrder();
        }

        private enum States
        {
            takingOrder,
            makingOrder,
            success,
            fail
        }

        private States State = States.takingOrder;

        private void TakingOrder() // Basically Idle
        {
            //bartender.animatedSprite.SetAnimation("bar_idle");
            Debug.Write("Wuh oh!! Evil scary Error");
        }

        private void MakingOrder()
        {

        }

    }
}
