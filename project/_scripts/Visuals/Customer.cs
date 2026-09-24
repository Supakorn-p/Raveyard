using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;
using Raveyard._scripts.Visuals;

namespace Raveyard;

public class Customer
{
    public Action orderFulfilled;

    public SpriteObject spriteObj;

    // positions will be hardcoded for now
    private static Vector2 startPos = new Vector2(-1000, 720/2 + 30);
    private static Vector2 presentPos = new Vector2(-200, 720/2 + 30);
    private static Vector2 endPos = new Vector2(1000, 720/2 + 30);

    private OrderBox orderbox = new OrderBox();

    public Customer(Texture2D _texture2D, Vector2 region)
    {
        spriteObj = new SpriteObject("customer", _texture2D, region, startPos);
        spriteObj.anchor = new Vector2(0.5f, 1f);
        orderbox.CreateOrderBoxSprite();

        spriteObj.LoadAnim("cus_talk", [1, 0], TimeSpan.FromMilliseconds(100), false);
    }

    public void StartOrder()
    {
        spriteObj.position = startPos;
        spriteObj.active = true;

        spriteObj.tweener.TweenTo(spriteObj, player => player.position, presentPos, 0.5f)
        .Easing(EasingFunctions.CubicInOut);

        orderbox.StartOrder();
    }

    public void AddToOrder(InputType input)
    {
        spriteObj.animatedSprite.SetAnimation("cus_talk");
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