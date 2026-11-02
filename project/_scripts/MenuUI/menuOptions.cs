using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Raveyard._scripts.MenuUI;
using Raveyard._scripts.MenuUI.ButtonTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace Raveyard;

public class menuOptions : ContextMenu
{
    private int selector = 0;
    private string[] validOptions = ["AUDIO_MUSIC", "AUDIO_SFX", "GRAPHICS_VSYNC", "GRAPHICS_FULLSCREEN", "GRAPHICS_APPLY"];
    private Dictionary<string, Action<int>> optionsFuncs = new Dictionary<string, Action<int>>()
    {
        ["AUDIO_MUSIC"] = (int direction) =>
        {
            SETTINGS.AUDIO_MUSIC_MULT = Math.Clamp(SETTINGS.AUDIO_MUSIC_MULT + 10 * direction, 0, 100);
            MediaPlayer.Volume = SETTINGS.AUDIO_MUSIC_MULT / 100f;
        },
        ["AUDIO_SFX"] = (int direction) =>
        {
            SETTINGS.AUDIO_SFX_MULT = Math.Clamp(SETTINGS.AUDIO_SFX_MULT + 10 * direction, 0, 100);
        },
        ["GRAPHICS_VSYNC"] = (int direction) =>
        {
            if (direction == 0) { return; }
            SETTINGS.GRAPHICS_VSYNC = !SETTINGS.GRAPHICS_VSYNC;
        },
        ["GRAPHICS_FULLSCREEN"] = (int direction) =>
        {
            if (direction == 0) { return; }
            SETTINGS.GRAPHICS_FULLSCREEN = !SETTINGS.GRAPHICS_FULLSCREEN;
        },
        ["GRAPHICS_APPLY"] = (int direction) =>
        {
            if (direction != 0) { return; }
            GameplaySoundLibrary.PlaySound("snd_click_menu_button");
            SETTINGS.ApplyGraphics();  
        },
    };

    private Dictionary<string, TextObject> optionsTextLabel = new Dictionary<string, TextObject>();

    private void ReconcileOptionsVisuals()
    {
        optionsTextLabel["AUDIO_MUSIC"].text = 
        new StringBuilder($"MUSIC VOL. : {SETTINGS.AUDIO_MUSIC_MULT}");
        optionsTextLabel["AUDIO_SFX"].text = 
        new StringBuilder($"SFX VOL. : {SETTINGS.AUDIO_SFX_MULT}");

        optionsTextLabel["GRAPHICS_VSYNC"].text = 
        new StringBuilder($"    VSYNC : {(SETTINGS.GRAPHICS_VSYNC ? "ON" : "OFF")}");
        optionsTextLabel["GRAPHICS_FULLSCREEN"].text = 
        new StringBuilder($"    FULLSCREEN : {(SETTINGS.GRAPHICS_FULLSCREEN ? "ON" : "OFF")}");

        optionsTextLabel["GRAPHICS_APPLY"].text = 
        new StringBuilder($"APPLY GRAPHICS");

        foreach (string opt in validOptions)
        {
            TextObject textObj = optionsTextLabel[opt];
            textObj.alpha = validOptions[selector] == opt ? 1.0f : 0.5f;
        }
    }

    private Vector2 startOptPos = new Vector2(1400, -200);
    private Vector2 startOptPosOffset = new Vector2(0, 64);
    private void InitializeOptions(SpriteFont font)
    {
        int i = 0;
        foreach (string opt in validOptions)
        {
            TextObject newTextObj = new TextObject(font, new StringBuilder("---"));
            newTextObj.position = startOptPos + startOptPosOffset * new Vector2(i, i);
            newTextObj.anchor = new Vector2(0f, 0.5f);
            newTextObj.color = Color.Black;
            newTextObj.layer = 5;
            newTextObj.active = true;
            newTextObj.scale = new Vector2(0.5f, 0.5f);
            optionsTextLabel.Add(opt, newTextObj);
            i += 1;
        }
        ReconcileOptionsVisuals();
    }

    private void SetCurrentOption(int direction)
    {
        string currentOption = validOptions[selector];
        optionsFuncs[currentOption](direction);
    }

    private void ScrollCurrentOption(int indexInc)
    {
        selector = (selector + indexInc) % validOptions.Count();
        if (selector < 0) { selector = validOptions.Count() - 1; }
    }

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
        InitializeOptions(Content.Load<SpriteFont>("anton_sc"));
    }

    private void InitButton(ContentManager Content)
    {
        centerButton = new SwitchMenu(true, new Vector2(0.5f, 0.5f));
        centerButton.parentMenu = this;
        centerButton.position = new Vector2(cameraPosition.X - 550, cameraPosition.Y);
        //centerButton.buttonBounds = new MonoGame.Extended.RectangleF(cameraPosition.X - 500, cameraPosition.Y + 300, 
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
    }

    public override void OnInputPressed(Keys key)
    {
        if (key == SETTINGS.keybind_back) { centerButton.ButtonPressed("snd_hover_menu_button"); GameplaySoundLibrary.PlaySound("snd_menu_woosh"); }

        if (key == Keys.Down) { ScrollCurrentOption(1); GameplaySoundLibrary.PlaySound("snd_hover_menu_button"); }
        if (key == Keys.Up) { ScrollCurrentOption(-1); GameplaySoundLibrary.PlaySound("snd_hover_menu_button"); }

        if (key == Keys.Left) { SetCurrentOption(-1); GameplaySoundLibrary.PlaySound("snd_hover_menu_button", 0.5f, 0f, -0.25f); }
        if (key == Keys.Right) { SetCurrentOption(1); GameplaySoundLibrary.PlaySound("snd_hover_menu_button", 0.5f, 0f, 0.25f); }
        if (key == Keys.Space) { SetCurrentOption(0); }
        

        ReconcileOptionsVisuals();
    }
}