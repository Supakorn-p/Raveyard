using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Raveyard;

public abstract class ContextMenu : IDisposable
{
    public ContextMenu switchOnExit;
    public Action onExit;

    public Vector2 cameraPosition {get; protected set;}

    // basic lifecycle funcs

    public virtual void Init(ContentManager Content) {}
    public virtual void Update(GameTime gameTime) {}
    public virtual void Dispose() {}

    public ContextMenu () {}
    public ContextMenu(ContextMenu menuToExitTo)
    {
        switchOnExit = menuToExitTo;
    }

    // when a menu is "focused", all inputs are directed to it

    public virtual void OnInputPressed(Keys key) {}
    public virtual void FocusedUpdate(GameTime gameTime) {}
    public virtual void OnFocusEnter() {}
    public virtual void OnFocusExit() {}

    protected void Exit() { onExit?.Invoke(); }
}