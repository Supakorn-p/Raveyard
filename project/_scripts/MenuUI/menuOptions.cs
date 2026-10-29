using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Raveyard._scripts.MenuUI;
using Raveyard._scripts.MenuUI.ButtonTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace Raveyard;

public class menuOptions : ContextMenu
{
    SpriteObject susie;

    SwitchMenu centerButton;
    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        cameraPosition = new Vector2(1408, 0);

        susie = new SpriteObject("menubg_opt", 
        Content.Load<Texture2D>("menu_options"), new Vector2(1280, 720),
        new Vector2(1408, 0));
        susie.scale = new Vector2(1.1f, 1.1f);
        susie.active = true;

        InitButton(Content);

    }

    private void InitButton(ContentManager Content)
    {
        centerButton = new SwitchMenu(true, new Vector2(0.25f, 0.25f));
        centerButton.parentMenu = this;
        centerButton.position = new Vector2(cameraPosition.X - 600, cameraPosition.Y);
        centerButton.buttonBounds = new MonoGame.Extended.RectangleF(cameraPosition.X - 500, cameraPosition.Y + 300, 
        500 * centerButton.scale.X, 500 * centerButton.scale.Y);

        centerButton.buttonSprite = new SpriteObject("testbutton",
           Content.Load<Texture2D>("test_button"), new Vector2(500, 500),
           centerButton.position);

        centerButton.buttonSprite.active = true;

        centerButton.ButtonInit();
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