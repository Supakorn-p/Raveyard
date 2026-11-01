using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Raveyard._scripts.MenuUI.ButtonTypes;
using Raveyard._scripts.MenuUI.Decorations;

namespace Raveyard;

public class menuCredits : ContextMenu
{
    SpriteObject susie;
    AllCredits allCreds;

    SwitchMenu centerButton;
    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        cameraPosition = new Vector2(-1536, 0);

        susie = new SpriteObject("menubg_cred", 
        Content.Load<Texture2D>("menu_credits"), new Vector2(1280, 720),
        new Vector2(-1472, 0));
        susie.scale = new Vector2(1.15f, 1.15f);
        susie.active = true;

        allCreds = new AllCredits();
        allCreds.parentMenu = this;
        allCreds.InitAllCreds(Content);

        InitButton(Content);
    }

    private void InitButton(ContentManager Content)
    {
        centerButton = new SwitchMenu(true, new Vector2(0.5f, 0.5f));
        centerButton.parentMenu = this;
        centerButton.position = new Vector2(cameraPosition.X + 550, cameraPosition.Y);
        //centerButton.buttonBounds = new MonoGame.Extended.RectangleF(cameraPosition.X + 500, cameraPosition.Y + 300,
        //500 * centerButton.scale.X, 500 * centerButton.scale.Y);

        centerButton.buttonSprite = new SpriteObject("testbutton",
           Content.Load<Texture2D>("menu_buttons_sprsheet"), new Vector2(600, 600),
           centerButton.position, 3);

        centerButton.buttonSprite.active = true;
        centerButton.buttonSprite.layer = 5;

        centerButton.ButtonInit(Content);

        centerButton.buttonLabel.SetText("BACK");
    }

    public override void FocusedUpdate(GameTime gameTime)
    {
        base.FocusedUpdate(gameTime);

        allCreds.Update(gameTime);
    }

    public override void OnInputPressed(Keys key)
    {
        if (key == SETTINGS.keybind_back) { centerButton.ButtonPressed("snd_hover_menu_button"); GameplaySoundLibrary.PlaySound("snd_menu_woosh"); }
    }
}