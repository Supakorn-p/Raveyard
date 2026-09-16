using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raveyard._scripts.Visuals
{
    public class InstructionKey
    {
        public SpriteObject instruction_key;
        public OrderBox box_owner;

        public Vector2 position;
        public Vector2 scale = new Vector2(0.25f, 0.25f);


        public void SetDistanceAndScale(InstructionKey inst_to_add)
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

                Debug.WriteLine((distanceMultiplier.X / box_owner.keyList.Count));
                key.instruction_key.position.X = (distanceMultiplier.X / box_owner.keyList.Count) - add_pos;
                key.instruction_key.position.Y = 220; //+ (box_owner.keyList.Count * 10);

                add_pos -= 100;


                //key.instruction_key.scale.X = 0.5f / box_owner.keyList.Count;
                //key.instruction_key.scale.Y = 0.5f / box_owner.keyList.Count;
            }
        }

        public void InitializeKey()
        {
            instruction_key.scale = scale;

            instruction_key.active = true;
        }
    }
}
