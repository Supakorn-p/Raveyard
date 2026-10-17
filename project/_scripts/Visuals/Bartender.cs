using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Tweening;
using Raveyard._scripts.Visuals;

namespace Raveyard._scripts.Characters
{
    public class Bartender
    {
        public SpriteObject bartender;
        public SpriteObject eyes;
        public Vector2 position;

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
            bartender.LoadAnim("bar_shake", [3, 4, 3, 4], TimeSpan.FromMilliseconds(120), false);
            bartender.LoadAnim("bar_shakeafter", [3, 4], TimeSpan.FromMilliseconds(240), true);
            bartender.LoadAnim("bar_finish", [5, 6, 5, 6, 5, 6], TimeSpan.FromMilliseconds(240), false);
            bartender.LoadAnim("bar_offbeat", [7, 8], TimeSpan.FromMilliseconds(240), true);
            bartender.LoadAnim("bar_offbeatbrief", [7, 8], TimeSpan.FromMilliseconds(100), false);
            bartender.LoadAnim("bar_fuckup", [9, 10, 9, 10, 9, 10], TimeSpan.FromMilliseconds(100), false); 

            eyes.LoadAnim("eye_idle", [1, 1, 1, 2], TimeSpan.FromMilliseconds(240), true);
            eyes.LoadAnim("eye_shake", [2], TimeSpan.FromMilliseconds(240), true);
            eyes.LoadAnim("eye_takingorder", [0, 0, 0, 2], TimeSpan.FromMilliseconds(240), true);
            eyes.LoadAnim("eye_perfect", [2], TimeSpan.FromMilliseconds(240), true);

            TakingOrder();
            OrderBox.inputsExhausted += PerfectOrder;

            // run functions after a specific animation finishes
            animationFinishExitStates.Add("bar_fuckup", () => TakingOrder());
            animationFinishExitStates.Add("bar_finish", () => TakingOrder());
            animationFinishExitStates.Add("bar_shake", () => SetAnimation("bar_shakeafter"));
            animationFinishExitStates.Add("bar_offbeatbrief", () => SetAnimation("bar_shakeafter"));
        }

        private Dictionary<string, Action> animationFinishExitStates = new Dictionary<string, Action>();

        private void OnBartenderAnimEnd(IAnimationController bart, AnimationEventTrigger trigger)
        {
            if (trigger != AnimationEventTrigger.AnimationCompleted) { return; }
            if (bart.IsLooping) { return; }
            string currentAnim = bartender.animatedSprite.CurrentAnimation;

            if (animationFinishExitStates.TryGetValue(currentAnim, out Action func))
            {
                func();
            }
        }


        private void TakingOrder() // Basically Idle
        {
            SetAnimation("bar_idle");
            SetEyeAnimation("eye_idle");
        }

        public void ActuallyTakingOrder()
        {
            SetAnimation("bar_takingorder");
            SetEyeAnimation("eye_takingorder");
        }

        private void PerfectOrder(bool isPerfect)
        {
            if (!isPerfect)
            {
                SetAnimation("bar_fuckup");
                SetEyeAnimation("eye_fuckup");
                return; 
            }
            SetAnimation("bar_finish");
            SetEyeAnimation("eye_perfect");
        }

        private void InputPressAnimation(InputType input)
        {
            bartender.tweener.CancelAll();
            eyes.tweener.CancelAll();

            Vector2 directionalOffset = Vector2.Zero;
            if (input == InputType.left) { directionalOffset = new Vector2(-10, 0); }
            if (input == InputType.right) { directionalOffset = new Vector2(10, 0); }

            bartender.position_offset = directionalOffset;
            eyes.position_offset = directionalOffset;

            bartender.tweener.TweenTo(bartender, player => player.position_offset, Vector2.Zero, 0.2f)
            .Easing(EasingFunctions.CubicOut);
            eyes.tweener.TweenTo(eyes, player => player.position_offset, Vector2.Zero, 0.2f)
           .Easing(EasingFunctions.CubicOut);

            if (input == InputType.press)
            {
                bartender.scale = new Vector2(1.0f, 0.95f);
                eyes.scale = new Vector2(1.0f, 0.95f);
                bartender.tweener.TweenTo(bartender, player => player.scale, new Vector2(1f, 1f), 0.2f)
                .Easing(EasingFunctions.CubicOut);
                eyes.tweener.TweenTo(eyes, player => player.scale, new Vector2(1f, 1f), 0.2f)
                .Easing(EasingFunctions.CubicOut);
            }
        }


        public void OnInputResult(JudgementResult result, InputType input)
        {
            // bounce regardless of input result
            InputPressAnimation(input);

            // unsuccessful inputs

            if (result == JudgementResult.none) { GameplaySoundLibrary.PlaySound("inputbeep"); return; }
            if (result == JudgementResult.miss) { 
                SetAnimation("bar_offbeat");
                SetEyeAnimation("eye_idle");
                GameplaySoundLibrary.PlaySound("inputmissed"); 
                return;
            }

            // successful inputs
            SetAnimation("bar_shake");
            SetEyeAnimation("eye_shake");

            bool isPerfect = result == JudgementResult.perfect;
            if (!isPerfect) // early/late
            {
                SetAnimation("bar_offbeatbrief");
            }

            float soundPitchOffset = isPerfect ? 0.0f : 0.5f;

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
            bartender.animatedSprite.Controller.OnAnimationEvent -= OnBartenderAnimEnd;
            bartender.animatedSprite.SetAnimation(animName).OnAnimationEvent += OnBartenderAnimEnd;
        }

        public void SetEyeAnimation(string animName)
        {
            if (animName == "eye_fuckup") { eyes.active = false; return; }
            eyes.active = true;
            eyes.animatedSprite.SetAnimation(animName);
        }
    }
}
