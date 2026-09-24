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
        public static Action<bool> inputsExhausted;

        public SpriteObject order_box;
        public Vector2 position;

        private Vector2 resting_pos = new Vector2(-1000, 200);
        public Vector2 start_pos = new Vector2(250, 200);
        private Vector2 end_pos = new Vector2(250, 1000);

        private Rectangle instructionsBoxRect = new Rectangle(
            100, 150, 
            330, 120);
        private int buttonsPerRow = 5;

        private Queue<InstructionKey> instructionsQueue = new Queue<InstructionKey>(10);
        private Bag<InstructionKey> instructionkeyList = new Bag<InstructionKey>(5);
        private static Texture2D instructionsTexture;
        //private Dictionary<InputType, Texture2D> instructionsTexture2D = new Dictionary<InputType, Texture2D>(3);

        private static Texture2D orderBoxTexture;

        public OrderBox()
        {
            position = resting_pos;
        }

        public static void InitializeOrderBox(Texture2D _orderBoxTexture, Texture2D _instructionsTexture)
        {
            orderBoxTexture = _orderBoxTexture;
            instructionsTexture = _instructionsTexture;
        }

        public void CreateOrderBoxSprite()
        {
            if (orderBoxTexture == null) { throw new Exception("initialize the order box's textures first!");}

            order_box = new SpriteObject("order_box", orderBoxTexture, new Vector2(800, 400), resting_pos);
            order_box.scale = new Vector2(0.7f, 0.7f);
            order_box.layer += 1;
            order_box.active = true;
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
            order_box.alpha = 0f;
            order_box.tweener.TweenTo(target: order_box, expression: player => player.position, toValue: start_pos, duration: 0.6f)
                .Easing(EasingFunctions.CubicInOut);
            order_box.tweener.TweenTo(target: order_box, expression: player => player.alpha, toValue: 1.0f, duration: 0.2f, delay: 0.3f)
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
            order_box.tweener.TweenTo(target: order_box, expression: player => player.scale, toValue: new Vector2(0.77f, 0.77f), duration: 0.15f)
               .Easing(EasingFunctions.CubicIn)
               .OnEnd(tween => order_box.tweener.TweenTo(target: order_box, expression: player => player.scale, toValue: new Vector2(0.75f, 0.75f), duration: 0.15f)
               .Easing(EasingFunctions.CubicOut));
            
            InstructionKey instruction = new InstructionKey();
            instruction.instruction_key = new SpriteObject("instruction", instructionsTexture,
            new Vector2(256, 256), Vector2.Zero, inputTypeToSprFrame[input]);
            instruction.instruction_key.layer = order_box.layer + 1;

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
            .OnEnd(tween => DestroyBox());

            inputsExhausted?.Invoke(isPerfect);
            if (isPerfect) {GameplaySoundLibrary.PlaySound("snd_result_cashregister");}
        }

        public void DestroyBox()
        {
            order_box.Free(); 
        }

        public bool RemoveInstruction(JudgementResult result)
        {
            if (instructionsQueue.Count <= 0) { return true; }
            InstructionKey removedKeySprite = instructionsQueue.Dequeue();

            removedKeySprite.instruction_key.scale *= new Vector2(0.75f, 0.75f);
            removedKeySprite.instruction_key.alpha = 0.75f;


            // skew when early/late
            if (result == JudgementResult.early || result == JudgementResult.late)
            {
                removedKeySprite.instruction_key.rotation = 5f;
                removedKeySprite.instruction_key.position += new Vector2(5, 5);
            }

            // miss
            if (result == JudgementResult.miss)
            {
                isPerfect = false;
                removedKeySprite.instruction_key.rotation = 15f;
                removedKeySprite.instruction_key.position += new Vector2(0, 10);
                removedKeySprite.instruction_key.animatedSprite.Color = Color.Green;
            }

            if (instructionsQueue.Count == 0)
            {
                foreach (InstructionKey key in instructionkeyList)
                {
                    key.TweenDown();
                }
                instructionkeyList.Clear();
                EndOrder();
                return true;
            }

            return false;
        }
    }
}
