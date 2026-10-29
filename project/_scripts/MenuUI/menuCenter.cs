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

namespace Raveyard;

public class menuCenter : ContextMenu
{
    private ContextMenu rightMenu;
    private ContextMenu leftMenu;
    private ContextMenu downMenu;

    private SwitchMenu buttonTest;

    private SwitchMenu buttonLeft;
    private SwitchMenu buttonRight;
    private SwitchMenu buttonDown;

    private List<SwitchMenu> buttonList = new List<SwitchMenu> { };

    public void SetMenus(ContextMenu right, ContextMenu left, ContextMenu down)
    {
        rightMenu = right; 
        leftMenu = left;
        downMenu = down;
    }

    SpriteObject susie;
    public SpriteObject titleText;
    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        cameraPosition = new Vector2(0, 0);

        susie = new SpriteObject("menubg", 
        Content.Load<Texture2D>("menu_center"), new Vector2(1280, 720),
        new Vector2(0, 0));
        susie.scale = new Vector2(1.15f, 1.15f);
        susie.active = true;

        titleText = new SpriteObject("title_text",
        Content.Load<Texture2D>("raveyard_text_menu"), new Vector2(1280, 720),
        new Vector2(0, 0));
        titleText.active = true;

        buttonLeft = new SwitchMenu(true, new Vector2(0.5f, 0.5f));
        buttonRight = new SwitchMenu(true, new Vector2(0.5f, 0.5f));
        buttonDown = new SwitchMenu(true, new Vector2(0.5f, 0.5f));

        buttonList.Add(buttonRight);
        buttonList.Add(buttonLeft);
        buttonList.Add(buttonDown);

        InitAllButtons(Content);

    }

    private void InitAllButtons(ContentManager Content)
    {
        int i = 0;
        buttonLeft.position = new Vector2(this.cameraPosition.X - 550, this.cameraPosition.Y);
        buttonRight.position = new Vector2(this.cameraPosition.X + 550, this.cameraPosition.Y);
        buttonDown.position = new Vector2(this.cameraPosition.X, this.cameraPosition.Y + 300);

        foreach (SwitchMenu button in buttonList)
        {
            button.parentMenu = this;
            button.buttonSprite = new SpriteObject("testbutton",
            Content.Load<Texture2D>("menu_buttons_sprsheet"), new Vector2(600, 600),
            button.position, i);

            button.buttonBounds = new RectangleF(button.position.X + 600, button.position.Y + 300, 500 * button.scale.X, 500 * button.scale.Y);

            button.buttonSprite.active = true;

            if (i == 0) { button.menuToSwitch = rightMenu; }
            if (i == 1) { button.menuToSwitch = leftMenu; }
            if (i == 2) { button.menuToSwitch = downMenu; }

            button.ButtonInit();
            i++;
        }
    }

    public override void FocusedUpdate(GameTime gameTime)
    {
        base.FocusedUpdate(gameTime);

        foreach (SwitchMenu button in buttonList)
        {
            button.Update(gameTime);
        }
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