using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;

namespace Raveyard;

// hardcoded mess for the playtest
public class placeholder_ResultsBubble
{
    public SpriteObject bubble;

    public TextObject score;

    public TextObject scoreLabel;
    public TextObject restartLabel;

    public void Init(Texture2D bubbleTexture, SpriteFont font)
    {
        bubble = new SpriteObject("score_bubble", bubbleTexture, new Vector2(800, 400),
        Vector2.Zero);

        bubble.anchor = new Vector2(0.52f, 0.55f);
        bubble.layer = 5;

        score = new TextObject(font, new System.Text.StringBuilder("100/100"));
        score.color = Color.Black;
        score.scale = new Vector2(1.8f, 1.8f);
        score.layer = 11;

        scoreLabel = new TextObject(font, new System.Text.StringBuilder("YOUR SCORE:"));
        scoreLabel.color = Color.Black;
        scoreLabel.scale = new Vector2(1f, 1f);
        scoreLabel.position = new Vector2(0, -200);
        scoreLabel.layer = 11;

        restartLabel = new TextObject(font, new System.Text.StringBuilder("PRESS [R] TO RESTART"));
        restartLabel.color = Color.Black;
        restartLabel.scale = new Vector2(0.5f, 0.5f);
        restartLabel.position = new Vector2(0, 300);
        restartLabel.layer = 11;
    }

    public void DisplayScore(int target)
    {
        bubble.active = true;
        score.active = true;
        scoreLabel.active = true;
        restartLabel.active = true;

        bubble.scale = new Vector2(1.1f, 3.5f);
        bubble.tweener.TweenTo(target: bubble, expression: player => player.scale, toValue: new Vector2(1f, 3.5f), duration: 2f)
        .Easing(EasingFunctions.CubicOut);

        targetScoreNum = target;
    }

    private double currentScoreNum = 0;
    private double targetScoreNum = 0;
    public void Update(GameTime gameTime)
    {
        currentScoreNum = MathHelper.Lerp((float) currentScoreNum, (float) targetScoreNum, 0.02f);
        score.text = new StringBuilder($"{Math.Round(currentScoreNum)}/100");
    }
}