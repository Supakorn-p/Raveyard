using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tweening;
using Raveyard._scripts.MenuUI;
using Raveyard._scripts.MenuUI.ButtonTypes;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;

namespace Raveyard;

public class menuLvlSelect : ContextMenu
{

    private Bag<SpriteObject> lvlCasetteSprites = new Bag<SpriteObject>(10);
    private Bag<TextObject> lvlCasetteTexts = new Bag<TextObject>(10);

    private const string casetteSelectSnd = "snd_cassette_select";

    private Vector2 casetteStartPos = new Vector2(-500, 65);
    private Vector2 casetteOffsetPos = new Vector2(100, 0);

    SwitchMenu centerButton;
    public bool inGame = false;

    DecorativeButton leftButton;
    DecorativeButton rightButton;
    DecorativeButton playButton;

    List<DecorativeButton> decorButtons = new List<DecorativeButton> { };

    private void LoadLevels(ContentManager Content)
    {
        foreach (LEVELS_metadata level in LEVELS.List)
        {
            // skip if the level's .raveyard file doesn't exist in _charts
            string raveyardPath = Path.ChangeExtension(LEVELS.GetLevelPathByName(level.fileName), "raveyard");
            if (!Path.Exists(raveyardPath)) { continue; }

            int offsetBy = lvlCasetteSprites.Count;

            SpriteObject newCasetteSprite = new SpriteObject("menubg_lvl", 
            Content.Load<Texture2D>("cassette_tape_sprsheet"), new Vector2(251, 456),
            cameraPosition + casetteStartPos + casetteOffsetPos * offsetBy);
            susie.scale = new Vector2(1.2f, 1.2f);

            newCasetteSprite.layer = 1;

            TextObject newCasetteText = new TextObject(Content.Load<SpriteFont>("anton_sc"), 
                                                       new StringBuilder(level.displayName));

            newCasetteText.anchor = new Vector2(0f, 0.5f);
            newCasetteText.position = newCasetteSprite.position + new Vector2(0, 150);
            newCasetteText.rotation = -90;
            newCasetteText.layer = 2;
            newCasetteText.scale = new Vector2(0.3f, 0.3f);

            newCasetteSprite.active = true;
            newCasetteText.active = true;

            lvlCasetteSprites.Add(newCasetteSprite);
            lvlCasetteTexts.Add(newCasetteText);
        }

        SelectTape(0);
    }

    private int selector;
    private void SelectTape(int index)
    {
        if (index >= lvlCasetteSprites.Count) { index = 0; }
        if (index < 0) { index = lvlCasetteSprites.Count - 1; }
        selector = index;

        // highlight
        int spr_txt = 0;
        foreach (SpriteObject sprite in lvlCasetteSprites)
        {
            if (spr_txt > lvlCasetteTexts.Count - 1) { break; }

            sprite.alpha = spr_txt == index ? 1f : 0.8f;
            //sprite.position = spr_txt == index ? new Vector2(sprite.position.X, cameraPosition.Y + casetteStartPos.Y - 50) 
            //: new Vector2(sprite.position.X, cameraPosition.Y + casetteStartPos.Y); 

            if (spr_txt == index)
            {
                GameplaySoundLibrary.PlaySound(casetteSelectSnd);
                sprite.tweener.TweenTo(target: sprite, player => player.position,
                   toValue: new Vector2(sprite.position.X, cameraPosition.Y + casetteStartPos.Y - 50), duration: 0.15f)
                   .Easing(EasingFunctions.CubicOut);


                lvlCasetteTexts[spr_txt].tweener.TweenTo(target: lvlCasetteTexts[spr_txt], player => player.position,
                   toValue: new Vector2(lvlCasetteTexts[spr_txt].position.X, cameraPosition.Y + casetteStartPos.Y + 100), duration: 0.15f)
                   .Easing(EasingFunctions.CubicOut);
            }
            else
            {
                sprite.tweener.TweenTo(target: sprite, player => player.position,
                   toValue: new Vector2(sprite.position.X, cameraPosition.Y + casetteStartPos.Y), duration: 0.25f)
                   .Easing(EasingFunctions.BounceOut);

                lvlCasetteTexts[spr_txt].tweener.TweenTo(target: lvlCasetteTexts[spr_txt], player => player.position,
                   toValue: new Vector2(lvlCasetteTexts[spr_txt].position.X, cameraPosition.Y + casetteStartPos.Y + 150), duration: 0.25f)
                   .Easing(EasingFunctions.BounceOut);
            }

            spr_txt++;
        }
    }

    private ScreenManager _screenManager;
    private Game _game;
    public menuLvlSelect(ScreenManager screenManager, Game game) { _screenManager = screenManager; _game = game; }

    private void PlayLevel()
    {
        if (selector >= LEVELS.List.Count()) { return; }

        MediaPlayer.Stop();
        inGame = true;

        LEVELS_metadata selectedLevel = LEVELS.List[selector];

        // return if the level's .wav file doesn't exist in _charts
        string audioPath = Path.ChangeExtension(LEVELS.GetLevelPathByName(selectedLevel.fileName), "wav");
        if (!Path.Exists(audioPath)) { return; }

        _screenManager.ReplaceScreen(new scGameplay(_game, selectedLevel.fileName, selectedLevel.levelParameters));
    }

    SpriteObject susie;
    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        cameraPosition = new Vector2(0, 792);

        susie = new SpriteObject("menubg_lvl", 
        Content.Load<Texture2D>("menu_screen_2"), new Vector2(1280, 720),
        new Vector2(0, 792));
        susie.active = true;

        new GameplaySoundLibrary(Content).LoadSound(casetteSelectSnd);

        InitButton(Content);
        LoadLevels(Content);
    }

    private void InitButton(ContentManager Content)
    {
        centerButton = new SwitchMenu(true, new Vector2(0.5f, 0.5f));
        centerButton.parentMenu = this;
        centerButton.position = new Vector2(cameraPosition.X, cameraPosition.Y - 275);
        centerButton.buttonBounds = new MonoGame.Extended.RectangleF(cameraPosition.X + 500, cameraPosition.Y + 300,
        500 * centerButton.scale.X, 500 * centerButton.scale.Y);

        centerButton.buttonSprite = new SpriteObject("testbutton",
           Content.Load<Texture2D>("menu_buttons_sprsheet"), new Vector2(600, 600),
           centerButton.position, 3);

        centerButton.buttonSprite.active = true;
        centerButton.buttonSprite.layer = 5;

        centerButton.ButtonInit(Content);

        centerButton.buttonLabel.position = new Vector2(centerButton.position.X, centerButton.position.Y + 100);
        centerButton.buttonLabel.SetText("BACK");


        leftButton = new DecorativeButton(true, new Vector2(0.35f, 0.35f));
        rightButton = new DecorativeButton(true, new Vector2(0.35f, 0.35f));
        playButton = new DecorativeButton(true, new Vector2(0.35f, 0.35f));

        leftButton.position = new Vector2(cameraPosition.X - 500, cameraPosition.Y + 300);
        rightButton.position = new Vector2(cameraPosition.X - 300, cameraPosition.Y + 300);
        playButton.position = new Vector2(cameraPosition.X, cameraPosition.Y + 300);


        decorButtons.Add(rightButton);
        decorButtons.Add(leftButton);
        decorButtons.Add(playButton);
        
        int i = 0;
        foreach (DecorativeButton button in decorButtons)
        {
            button.buttonSprite = new SpriteObject("testbutton",
           Content.Load<Texture2D>("menu_buttons_sprsheet"), new Vector2(600, 600),
           button.position, i);

            button.buttonSprite.layer = 5;
            button.buttonSprite.active = true;

            button.ButtonInit(Content);

            button.buttonLabel.scale = new Vector2(0.35f, 0.35f);

            if (i == 0) { button.buttonLabel.SetText("SELECT"); button.buttonLabel.position = new Vector2(button.position.X - 100, button.position.Y); }
            if (i == 2) { button.buttonLabel.SetText("PLAY"); button.buttonLabel.position = new Vector2(button.position.X, button.position.Y - 40); }

            i++;
        }
    }

    public override void FocusedUpdate(GameTime gameTime)
    {
        base.FocusedUpdate(gameTime);
    }

    public override void OnInputPressed(Keys key)
    {
        if (key == SETTINGS.keybind_back) { centerButton.ButtonPressed("snd_hover_menu_button"); GameplaySoundLibrary.PlaySound("snd_menu_woosh"); }

        if (key == Keys.Left) { SelectTape(selector - 1); leftButton.ButtonPressed("snd_hover_menu_button"); }
        if (key == Keys.Right) { SelectTape(selector + 1); rightButton.ButtonPressed("snd_hover_menu_button"); }

        if (key == Keys.Space) { PlayLevel(); }
    }
}