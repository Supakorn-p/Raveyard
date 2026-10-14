using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Raveyard;

public class menuCredits : ContextMenu
{
    SpriteObject susie;
    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        cameraPosition = new Vector2(-1280, 0);

        susie = new SpriteObject("menubg_cred", 
        Content.Load<Texture2D>("menu_bgcredits"), new Vector2(1280, 720),
        new Vector2(-1280, 0));
        susie.active = true;
    }

    public override void FocusedUpdate(GameTime gameTime)
    {
        base.FocusedUpdate(gameTime);
    }

    public override void OnInputPressed(Keys key)
    {
        if (key == SETTINGS.keybind_back) { Exit(); }
    }
}