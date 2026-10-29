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
        cameraPosition = new Vector2(-1536, 0);

        susie = new SpriteObject("menubg_cred", 
        Content.Load<Texture2D>("menu_credits"), new Vector2(1280, 720),
        new Vector2(-1472, 0));
        susie.scale = new Vector2(1.15f, 1.15f);
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