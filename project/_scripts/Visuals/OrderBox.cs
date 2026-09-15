using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raveyard._scripts.Visuals
{
    public class OrderBox
    {
        public SpriteObject order_box;
        public Vector2 position;

        public readonly Tweener tweener = new Tweener();
        private Vector2 resting_pos = new Vector2(1300, 300);
        private Vector2 start_pos = new Vector2(300, 300);
        private Vector2 end_pos = new Vector2(300, 1000);

        private int instructionsLeft = 0; // TODO: replace this with a queue that keeps track of each instruction

        public OrderBox()
        {
            position = resting_pos;
        }

        public void InitializeOrderBox()
        {
            order_box.scale = new Vector2(0.8f, 0.8f);
        }

        public void StartOrder()
        {
            order_box.position = resting_pos;
            order_box.tweener.TweenTo(target: order_box, expression: player => player.position, toValue: start_pos, duration: 1)
                .Easing(EasingFunctions.CubicInOut);
        }


        public void InstructionAdded()
        {
            order_box.tweener.TweenTo(target: order_box, expression: player => player.scale, toValue: new Vector2(0.82f, 0.82f), duration: 0.15f)
               .Easing(EasingFunctions.CubicIn)
               .OnEnd(tween => order_box.tweener.TweenTo(target: order_box, expression: player => player.scale, toValue: new Vector2(0.8f, 0.8f), duration: 0.15f)
               .Easing(EasingFunctions.CubicOut));

            instructionsLeft += 1; // TODO: replace this, see variable itself
        }


        public void EndOrder()
        {
            order_box.tweener.TweenTo(target: order_box, expression: player => player.position, toValue: end_pos, duration: 1)
                .Easing(EasingFunctions.CubicInOut)
                .OnEnd(tween => WaitForOrder());
        }

        public void WaitForOrder()
        {
            order_box.position = resting_pos; 
        }

        public void RemoveInstruction()
        {
            instructionsLeft -= 1; // TODO: replace this, see variable itself
            if (instructionsLeft == 0)
            {
                EndOrder();
            }
        }
    }
}
