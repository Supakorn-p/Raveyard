using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Screens;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Raveyard;

public class menuLvlSelect : ContextMenu
{

    private Bag<SpriteObject> lvlCasetteSprites = new Bag<SpriteObject>(10);
    private Bag<TextObject> lvlCasetteTexts = new Bag<TextObject>(10);

    private Vector2 casetteStartPos = new Vector2(-500, 65);
    private Vector2 casetteOffsetPos = new Vector2(100, 0);

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
        if (index >= lvlCasetteSprites.Count) {index = 0;}
        if (index < 0) {index = lvlCasetteSprites.Count - 1;}
        selector = index;

        // highlight
        int spr_txt = 0;
        foreach (SpriteObject sprite in lvlCasetteSprites)
        {
            if (spr_txt > lvlCasetteTexts.Count - 1) { break; }

            sprite.alpha = spr_txt == index ? 1f : 0.8f;
            sprite.position = spr_txt == index ? new Vector2(sprite.position.X, cameraPosition.Y + casetteStartPos.Y - 50) : new Vector2(sprite.position.X, cameraPosition.Y + casetteStartPos.Y);
            spr_txt++;
        }
    }

    private ScreenManager _screenManager;
    private Game _game;
    public menuLvlSelect(ScreenManager screenManager, Game game) { _screenManager = screenManager; _game = game; }

    private void PlayLevel()
    {
        if (selector >= LEVELS.List.Count()) { return; }

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

        LoadLevels(Content);
    }

    public override void FocusedUpdate(GameTime gameTime)
    {
        base.FocusedUpdate(gameTime);
    }

    public override void OnInputPressed(Keys key)
    {
        if (key == SETTINGS.keybind_back) { Exit(); }

        if (key == Keys.Left) { SelectTape(selector - 1); }
        if (key == Keys.Right) { SelectTape(selector + 1); }

        if (key == Keys.Space) { PlayLevel(); }
    }
}