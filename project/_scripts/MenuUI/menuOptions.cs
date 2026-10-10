using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Raveyard;

public class menuOptions : ContextMenu
{
    SpriteObject susie;
    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        cameraPosition = new Vector2(500, 0);

        susie = new SpriteObject("susie", 
        Content.Load<Texture2D>("placeholder"), new Vector2(640, 640),
        new Vector2(500, 0));
        susie.scale = new Vector2(0.5f, 0.5f);
        susie.active = true;
    }

    public override void FocusedUpdate(GameTime gameTime)
    {
        base.FocusedUpdate(gameTime);
        
        if (Keyboard.GetState().IsKeyDown(Keys.Q)) { susie.rotation += -0.5f; }
        if (Keyboard.GetState().IsKeyDown(Keys.E)) { susie.rotation += 0.5f; }
    }

    public override void OnInputPressed(Keys key)
    {
        if (key == SETTINGS.keybind_back) { Exit(); }
    }

    public override void OnFocusEnter() { susie.alpha = 1f; }
    public override void OnFocusExit() { susie.alpha = 0.5f; }
}