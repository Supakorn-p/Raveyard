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

        public Vector2 position;
        public Vector2 scale = new Vector2(0.25f, 0.25f);

        private Dictionary<InputType, SpriteObject> inputTypes = new Dictionary<InputType, SpriteObject>();


        public void InitializeKey()
        {
            instruction_key.scale = scale;

            instruction_key.active = true;
        }
    }
}
