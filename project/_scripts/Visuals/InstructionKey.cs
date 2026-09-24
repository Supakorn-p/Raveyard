using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;


namespace Raveyard._scripts.Visuals
{
    public class InstructionKey
    {
        /* 
        feedback: reading from box_owner directly isn't necessary, the instructions sprite can infer its position
        from the position it was created at
        */
        public SpriteObject instruction_key;
        //public OrderBox box_owner;
        public Vector2 scale = new Vector2(0.3f, 0.3f);
        public int indexNumber = 0;

        /*public void SetDistanceAndScale(InstructionKey inst_to_add)
        {
            int add_pos = 0;
            float sub_scale = 0;
            int key_index = 0;
           // float distanceMultiplier = 1;

            Vector2 distanceMultiplier = new Vector2(0,0);
            Vector2 scaleMultiplier = new Vector2(0,0);
            box_owner.keyList.Add(inst_to_add);

            foreach (InstructionKey key in box_owner.keyList)
            {
                key_index += 1;

                distanceMultiplier.X += 150;
                distanceMultiplier.Y += 5;


                scaleMultiplier.X += 0.01f;
                scaleMultiplier.Y += 0.01f;

                //Debug.WriteLine((distanceMultiplier.X / box_owner.keyList.Count));
                key.instruction_key.position.X = (distanceMultiplier.X / box_owner.keyList.Count) - add_pos;
                key.instruction_key.position.Y = 220; //+ (box_owner.keyList.Count * 10);

                add_pos -= 100;


                //key.instruction_key.scale.X = 0.5f / box_owner.keyList.Count;
                //key.instruction_key.scale.Y = 0.5f / box_owner.keyList.Count;
            }
        }*/
        public void UpdatePosition(Rectangle boundaries, int _instPerRow)
        {
            int instPerRow = _instPerRow - 1;
            float yCenter = boundaries.Y + boundaries.Height/2f;
            float xCenter = boundaries.X + boundaries.Width/2f;

            float xOffset = (indexNumber - instPerRow/2f)/(instPerRow/2f);
            instruction_key.position = new Vector2(xCenter + xOffset * boundaries.Width/2f, yCenter);
        }

        public void TweenDown()
        {
            instruction_key.tweener.TweenTo(target: instruction_key, player => player.position, 
            toValue: instruction_key.position + new Vector2(0, 800), duration: 1)
            .Easing(EasingFunctions.CubicInOut).OnEnd((Tween t) =>
            {
                instruction_key.Free();
            });
        }

        public void InitializeKey(int n)
        {
            indexNumber = n;
            instruction_key.scale = scale;
            instruction_key.active = true;
        }
    }
}
