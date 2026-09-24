using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Tweening;
using Raveyard._scripts.Visuals;

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
            bartender.LoadAnim("bar_shake", [2, 2, 3, 3], TimeSpan.FromMilliseconds(240), true);
            bartender.LoadAnim("bar_finish", [4, 4, 5, 5], TimeSpan.FromMilliseconds(240), true); 
            bartender.LoadAnim("bar_fuckup", [6], TimeSpan.FromMilliseconds(240), true); 

            TakingOrder();
            OrderBox.inputsExhausted += PerfectOrder;
        }


        private void TakingOrder() // Basically Idle
        {
            SetAnimation("bar_idle");
        }

        private void PerfectOrder(bool isPerfect)
        {
            if (!isPerfect) { return; }
            SetAnimation("bar_finish");
        }

        public void OnInputResult(JudgementResult result, InputType input)
        {
            // bounce regardless of input result
            SetAnimation("bar_idle");
            bartender.tweener.CancelAll();
            bartender.scale = new Vector2(1.0f, 0.9f);
            bartender.tweener.TweenTo(bartender, player => player.scale, new Vector2(1f, 1f), 0.2f)
            .Easing(EasingFunctions.CubicOut);

            // unsuccessful inputs

            if (result == JudgementResult.none) { GameplaySoundLibrary.PlaySound("inputbeep"); return; }
            if (result == JudgementResult.miss) { 
                SetAnimation("bar_fuckup");
                GameplaySoundLibrary.PlaySound("inputmissed"); 
                return; 
            }

            // successful inputs
            bartender.animatedSprite.SetAnimation("bar_shake");
            float soundPitchOffset = result == JudgementResult.perfect ? 0.0f : 0.5f;

            if (input == InputType.press)
            {
                GameplaySoundLibrary.PlaySound("snd_input_glassclink", 1.0f, 0.0f, soundPitchOffset);
            }
            if (input == InputType.left)
            {
                GameplaySoundLibrary.PlaySound("snd_input_shakeleft", 1.0f, 0.0f, soundPitchOffset);
            }
            if (input == InputType.right)
            {
                GameplaySoundLibrary.PlaySound("snd_input_shakeright", 1.0f, 0.0f, soundPitchOffset);
            }
        }

        public void SetAnimation(string animName)
        {
            bartender.animatedSprite.SetAnimation(animName);
        }
    }
}
