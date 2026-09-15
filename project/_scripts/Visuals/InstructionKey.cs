using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raveyard._scripts.Visuals
{
    public class InstructionKey
    {
        public SpriteObject space;
        public SpriteObject left;
        public SpriteObject right;

        public SpriteObject instruction_key;

        public Vector2 position;
        public Vector2 scale = new Vector2(0.5f, 0.5f);
        public enum InstructionType
        {
            Space,
            Left,
            Right
        }

        public InstructionType keyType;

        private Dictionary<InstructionType, SpriteObject> insDict = new Dictionary<InstructionType, SpriteObject>();

        public InstructionKey(InstructionType _keyType)
        {
            // add all controls into the dictionary
            insDict.Add(InstructionType.Space, space);
            insDict.Add(InstructionType.Left, left);
            insDict.Add(InstructionType.Right, right);

            keyType = _keyType;

            instruction_key = insDict[keyType];
            
        }

    }
}
