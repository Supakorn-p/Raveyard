using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;
using Raveyard._scripts.Visuals;

namespace Raveyard;

public class TwinHead
{
    public Action orderFulfilled;

    public SpriteObject spriteObj;

    // positions will be hardcoded for now
    private static Vector2 startPos = new Vector2(-1000, 720/2 + 30);
    private static Vector2 presentPos = new Vector2(-200, 720/2 + 30);
    private static Vector2 endPos = new Vector2(1000, 720/2 + 30);

    private OrderBox orderbox = new OrderBox(false);
    private OrderBox orderbox2 = new OrderBox(true);

    public TwinHead(Texture2D _texture2D, Vector2 region)
    {
        spriteObj = new SpriteObject("twinhead", _texture2D, region, startPos);
        spriteObj.anchor = new Vector2(0.5f, 1f);
        orderbox.CreateOrderBoxSprite();
        orderbox2.CreateOrderBoxSprite();

        spriteObj.LoadAnim("th_talk", [1, 0], TimeSpan.FromMilliseconds(100), false);
        spriteObj.LoadAnim("th_talk2", [3, 2], TimeSpan.FromMilliseconds(100), false);
    }

    public void StartOrder()
    {
        spriteObj.position = startPos;
        spriteObj.active = true;

        spriteObj.tweener.TweenTo(spriteObj, player => player.position, presentPos, 0.5f)
        .Easing(EasingFunctions.CubicInOut);

        orderbox.StartOrder();
    }

    public void StartAnotherOrder()
    {
        orderbox2.StartOrder();
    }

    public void AddToOrder(InputType input)
    {
        spriteObj.animatedSprite.SetAnimation("th_talk");
        orderbox.InstructionAdded(input);
    }

    public void AddToOrder2(InputType input)
    {
        spriteObj.animatedSprite.SetAnimation("th_talk2");
        orderbox.InstructionAdded(input);
    }

    public void ProcessOrder(JudgementResult result)
    {
        bool isEmpty = orderbox.RemoveInstruction(result);
        if (isEmpty) { EndOrder(); }
    }

    private void EndOrder()
    {
        orderFulfilled?.Invoke();
        orderFulfilled = null;
        
        spriteObj.tweener.TweenTo(spriteObj, player => player.position, endPos, 1f)
        .Easing(EasingFunctions.CubicInOut).OnEnd(_ => spriteObj.Free());
    }

    public void visual_offsetBoxLayer(int n)
    {
        orderbox.order_box.layer += n;
    }
}