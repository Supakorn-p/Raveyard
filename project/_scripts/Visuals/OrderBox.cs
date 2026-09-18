using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;

namespace Raveyard._scripts.Visuals
{
    public class OrderBox
    {
        /* 
        feedback: rather than having a dictionary of *names*, and having to load the textures,
        it's better to have Gameplay preload the textures for you, then pass Texture2Ds to the same dictionary instead
        */
        //public scGameplay main_game;
        public Action<bool> inputsExhausted;

        public SpriteObject order_box;
        public Vector2 position;

        private Vector2 resting_pos = new Vector2(1300, 200);
        public Vector2 start_pos = new Vector2(300, 200);
        private Vector2 end_pos = new Vector2(300, 1000);

        private Rectangle instructionsBoxRect = new Rectangle(
            150, 150, 
            330, 120);
        private int buttonsPerRow = 5;

        private Queue<InstructionKey> instructionsQueue = new Queue<InstructionKey>(10);
        private Bag<InstructionKey> instructionkeyList = new Bag<InstructionKey>(5);
        private Texture2D instructionsTexture;
        //private Dictionary<InputType, Texture2D> instructionsTexture2D = new Dictionary<InputType, Texture2D>(3);

        public OrderBox()
        {
            position = resting_pos;
        }

        public void InitializeOrderBox(Texture2D _instructionsTexture)
        {
            instructionsTexture = _instructionsTexture;
            order_box.scale = new Vector2(0.8f, 0.8f);
        }

        /*public void LoadInstructionsTexture(InputType inputType, Texture2D texture)
        {
            instructionsTexture2D.Add(inputType, texture);
        }*/

        private bool isPerfect = false;

        public void StartOrder()
        {
            isPerfect = true;

            order_box.tweener.CancelAll();
            order_box.position = resting_pos;
            order_box.tweener.TweenTo(target: order_box, expression: player => player.position, toValue: start_pos, duration: 0.5f)
                .Easing(EasingFunctions.CubicInOut);
        }


        //private Dictionary<InputType, String> inputTypes = new Dictionary<InputType, String> { {InputType.press, "Sapcebar-Icon" }, {InputType.left, "Left-Icon"}, {InputType.right, "Right-Icon" } };
        private Dictionary<InputType, int> inputTypeToSprFrame = new Dictionary<InputType, int>
        {
          {InputType.left, 0},
          {InputType.right, 1},
          {InputType.press, 2},
        };
        public void InstructionAdded(InputType input)
        {
            order_box.tweener.TweenTo(target: order_box, expression: player => player.scale, toValue: new Vector2(0.82f, 0.82f), duration: 0.15f)
               .Easing(EasingFunctions.CubicIn)
               .OnEnd(tween => order_box.tweener.TweenTo(target: order_box, expression: player => player.scale, toValue: new Vector2(0.8f, 0.8f), duration: 0.15f)
               .Easing(EasingFunctions.CubicOut));
            
            InstructionKey instruction = new InstructionKey();
            instruction.instruction_key = new SpriteObject("instruction", instructionsTexture,
            new Vector2(256, 256), Vector2.Zero, inputTypeToSprFrame[input]);

            instruction.InitializeKey(instructionsQueue.Count);
            instructionsQueue.Enqueue(instruction);
            
            instructionkeyList.Add(instruction);
            foreach (InstructionKey key in instructionkeyList) { key.UpdatePosition(instructionsBoxRect, buttonsPerRow); }
        }


        public void EndOrder()
        {
            order_box.tweener.CancelAll();
            order_box.tweener.TweenTo(target: order_box, expression: player => player.position, toValue: end_pos, duration: 1)
            .Easing(EasingFunctions.CubicInOut)
            .OnEnd(tween => WaitForOrder());

            inputsExhausted?.Invoke(isPerfect);
            if (isPerfect) {GameplaySoundLibrary.PlaySound("snd_result_cashregister");}
        }

        public void WaitForOrder()
        {
            order_box.position = resting_pos; 
        }

        public void RemoveInstruction(JudgementResult result)
        {
            if (instructionsQueue.Count <= 0) { return; }
            InstructionKey removedKeySprite = instructionsQueue.Dequeue();

            removedKeySprite.instruction_key.scale *= new Vector2(0.75f, 0.75f);
            removedKeySprite.instruction_key.animatedSprite.Alpha = 0.75f;


            // skew when early/late
            if (result == JudgementResult.early || result == JudgementResult.late)
            {
                removedKeySprite.instruction_key.rotation = 10f;
            }

            // miss
            if (result == JudgementResult.miss)
            {
                isPerfect = false;
                removedKeySprite.instruction_key.rotation = 15f;
                removedKeySprite.instruction_key.animatedSprite.Color = Color.Green;
            }

            if (instructionsQueue.Count == 0)
            {
                foreach (InstructionKey key in instructionkeyList)
                {
                    key.instruction_key.active = false;
                    key.instruction_key.Free();
                }
                instructionkeyList.Clear();
                EndOrder();
            }
        }
    }
}
