using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raveyard._scripts.Visuals.EventObjects.Distractions
{
    internal class Coughing : EventObject
    {
        OrderBox currentBox;
        public override void Init(ContentManager Content)
        {
            base.Init(Content);
            eventName = "dst_cough";
        }

        public override void OnEvent(string[] eventParams)
        {
            base.OnEvent(eventParams);

        }
    }
}
