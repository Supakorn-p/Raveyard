using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;

namespace Raveyard;

// hardcoded mess for the playtest (formerly)

public class ResultsBubble : EventObject
{
    public NinepatchSpriteObject bubble;

    public TextObject score;
    public TextObject scoreLabel;
    public TextObject restartLabel;

    private ScoreTracker scoreTracker;
    public void SetScoreTracker(ScoreTracker _scoreTracker) { scoreTracker = _scoreTracker; }

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "display_score";

        Texture2D bubbleTexture = Content.Load<Texture2D>("Dialogue-Box");
        SpriteFont font = Content.Load<SpriteFont>("anton_sc");

        bubble = new NinepatchSpriteObject("results_ui", 
        Content.Load<Texture2D>("ui_ninepatch_dark"), new Vector2(500, 100), new Vector2(0, -1400));
        bubble.box_border_size = 64;

        bubble.layer = 5;

        score = new TextObject(font, new StringBuilder("100/100"));
        score.color = Color.White;
        score.scale = new Vector2(1.8f, 1.8f);
        score.layer = 11;

        scoreLabel = new TextObject(font, new StringBuilder("CLEAR!"));
        scoreLabel.color = Color.White;
        scoreLabel.scale = new Vector2(1f, 1f);
        scoreLabel.position = new Vector2(0, -1400);
        scoreLabel.layer = 11;

        restartLabel = new TextObject(font, new StringBuilder("PRESS [Esc] TO RETURN"));
        restartLabel.color = Color.White;
        restartLabel.scale = new Vector2(0.5f, 0.5f);
        restartLabel.position = new Vector2(0, 300);
        restartLabel.layer = 11;
    }



    // public void DisplayScore(int target)
    // {
    //     bubble.active = true;
    //     score.active = true;
    //     scoreLabel.active = true;
    //     restartLabel.active = true;

    //     bubble.scale = new Vector2(1.1f, 3.5f);
    //     bubble.tweener.TweenTo(target: bubble, expression: player => player.scale, toValue: new Vector2(1f, 3.5f), duration: 2f)
    //     .Easing(EasingFunctions.CubicOut);

    //     targetScoreNum = target;
    // }

    private void DisplayScore()
    {
        scoreLabel.text = new StringBuilder("YOUR SCORE:");
        scoreLabel.tweener.TweenTo(target: scoreLabel, expression: player => player.position, 
        toValue: new Vector2(0, -170), duration: 0.25f)
        .Easing(EasingFunctions.CubicOut);

        bubble.tweener.TweenTo(target: bubble, expression: player => player.box_size, toValue: new Vector2(500, 720), duration: 0.25f)
        .Easing(EasingFunctions.CubicOut).OnEnd((tweener) => {
            currentScoreNum = -1;
            targetScoreNum = Math.Floor(scoreTracker.GetFinalPercentage()); 
            score.active = true;
        });
    }



    private void DisplayMenu()
    {
        scoreLabel.active = true;
        bubble.active = true;

        bubble.tweener.TweenTo(target: bubble, expression: player => player.position, toValue: Vector2.Zero, duration: 1f)
        .Easing(EasingFunctions.BounceOut);
        scoreLabel.tweener.TweenTo(target: scoreLabel, expression: player => player.position, toValue: Vector2.Zero, duration: 1f)
        .Easing(EasingFunctions.BounceOut).OnEnd((tweener) =>
        {
            DisplayScore();
        });
    }

    private void OrchestraHit()
    {
        GameplaySoundLibrary.PlaySound("snd_result_menu");
        score.scale = new Vector2(2f, 2f);
        score.tweener.TweenTo(target: score, expression: player => player.scale, toValue: new Vector2(1.8f, 1.8f), duration: 2f)
        .Easing(EasingFunctions.CubicOut);

        restartLabel.active = true;
    }

    public override void OnEvent(string[] eventParams)
    {
        base.OnEvent(eventParams);
        DisplayMenu();
    }

    private double currentScoreNum = 0;
    private double targetScoreNum = 0;

    private int _p_currentScoreNum = 0;
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        currentScoreNum = MathHelper.Lerp((float) currentScoreNum, (float) targetScoreNum, Math.Clamp(1f * delta, 0f, 1f));
        score.text = new StringBuilder($"{Math.Round(currentScoreNum)}%");

        if (_p_currentScoreNum != Math.Round(currentScoreNum))
        { 
            GameplaySoundLibrary.PlaySound("inputbeep", 1f, 0, (float) (currentScoreNum/targetScoreNum)); 
            if (Math.Round(currentScoreNum) == (int) targetScoreNum)
            {
                OrchestraHit();
            }
        }
        _p_currentScoreNum = (int) Math.Round(currentScoreNum);
    }
}