using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace Raveyard;

public class menuCenter : ContextMenu
{
    private ContextMenu rightMenu;
    private ContextMenu leftMenu;
    private ContextMenu downMenu;

    public void SetMenus(ContextMenu right, ContextMenu left, ContextMenu down)
    {
        rightMenu = right; 
        leftMenu = left;
        downMenu = down;
    }

    SpriteObject susie;
    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        cameraPosition = new Vector2(0, 0);

        susie = new SpriteObject("menubg", 
        Content.Load<Texture2D>("menu_bgmain"), new Vector2(1280, 720),
        new Vector2(0, 0));
        susie.scale = new Vector2(1.2f, 1.2f);
        susie.active = true;
    }

    public override void FocusedUpdate(GameTime gameTime)
    {
        base.FocusedUpdate(gameTime);
    }

    public override void OnInputPressed(Keys key)
    {
        if (key == Keys.Right) 
        { 
            switchOnExit = rightMenu;
            Exit(); 
        }
        if (key == Keys.Left) 
        { 
            switchOnExit = leftMenu;
            Exit(); 
        }
        if (key == Keys.Space) 
        { 
            switchOnExit = downMenu;
            Exit(); 
        }
    }
}