using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Raveyard;

public class EventObject
{
    public string eventName {get; protected set;} = "undefined";

    public virtual void Init(ContentManager Content) {}
    public virtual void Update(GameTime gameTime) {}

    public virtual void OnEvent(string[] eventParams) {}
}