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
        public SpriteObject eyes;
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
            position = new Vector2(125, 130);
        }

        public void BartenderInitialize()
        {
            bartender.anchor = new Vector2(0.5f, 1f);
            eyes.anchor = new Vector2(0.5f, 1f);
            bartender.LoadAnim("bar_idle", [0], TimeSpan.FromMilliseconds(240), true); // Idle
            bartender.LoadAnim("bar_takingorder", [1, 2], TimeSpan.FromMilliseconds(240), true);
            bartender.LoadAnim("bar_shake", [3, 3, 4, 4], TimeSpan.FromMilliseconds(240), true);
            bartender.LoadAnim("bar_finish", [5, 5, 6, 6], TimeSpan.FromMilliseconds(240), true);
            bartender.LoadAnim("bar_offbeat", [7, 8], TimeSpan.FromMilliseconds(240), true);
            bartender.LoadAnim("bar_fuckup", [9, 10], TimeSpan.FromMilliseconds(240), true); 

            eyes.LoadAnim("eye_idle", [1, 1, 1, 2], TimeSpan.FromMilliseconds(240), true);
            eyes.LoadAnim("eye_shake", [2], TimeSpan.FromMilliseconds(240), true);
            eyes.LoadAnim("eye_takingorder", [0, 0, 0, 2], TimeSpan.FromMilliseconds(240), true);
            eyes.LoadAnim("eye_perfect", [2], TimeSpan.FromMilliseconds(240), true);

            TakingOrder();
            OrderBox.inputsExhausted += PerfectOrder;
        }


        private void TakingOrder() // Basically Idle
        {
            SetAnimation("bar_idle");
            SetEyeAnimation("eye_idle");
        }

        private void ActuallyTakingOrder()
        {
            SetAnimation("bar_takingorder");
            SetEyeAnimation("eye_takingorder");
        }

        private void PerfectOrder(bool isPerfect)
        {
            if (!isPerfect) { return; }
            SetAnimation("bar_finish");
            SetEyeAnimation("eye_perfect");
        }

        public void OnInputResult(JudgementResult result, InputType input)
        {
            // bounce regardless of input result
            SetAnimation("bar_idle");
            SetEyeAnimation("eye_idle");
            bartender.tweener.CancelAll();
            eyes.tweener.CancelAll();

            bartender.scale = new Vector2(1.0f, 0.9f);
            eyes.scale = new Vector2(1.0f, 0.9f);

            bartender.tweener.TweenTo(bartender, player => player.scale, new Vector2(1f, 1f), 0.2f)
            .Easing(EasingFunctions.CubicOut);
            eyes.tweener.TweenTo(eyes, player => player.scale, new Vector2(1f, 1f), 0.2f)
           .Easing(EasingFunctions.CubicOut);

            // unsuccessful inputs

            if (result == JudgementResult.none) { GameplaySoundLibrary.PlaySound("inputbeep"); return; }
            if (result == JudgementResult.miss) { 
                SetAnimation("bar_offbeat");
                SetEyeAnimation("eye_shake");
                GameplaySoundLibrary.PlaySound("inputmissed"); 
                return;
            }

                // successful inputs
                bartender.animatedSprite.SetAnimation("bar_shake");
                SetEyeAnimation("eye_shake");
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

        public void SetEyeAnimation(string animName)
        {
            eyes.animatedSprite.SetAnimation(animName);
        }
    }
}
