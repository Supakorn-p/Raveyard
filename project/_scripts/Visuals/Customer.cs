 using System;
using System.Collections.Generic;
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
    private static Vector2 endPos = new Vector2(3000, 720/2 + 30);

    public OrderBox orderbox = new OrderBox();
    private DrinkWanted drinkWanted = new DrinkWanted(); 

    public Customer(Texture2D _texture2D, CustomerVariantData customerVariant)
    {
        spriteObj = new SpriteObject("customer", _texture2D, customerVariant.texture_region, startPos);
        spriteObj.anchor = new Vector2(0.5f, 1f);
        spriteObj.layer = 1;
        drinkWanted.CreateDrinkSprite();
        orderbox.CreateOrderBoxSprite();

        spriteObj.LoadAnim("cus_talk", customerVariant.frames_talk, TimeSpan.FromMilliseconds(100), false);
        spriteObj.LoadAnim("cus_satisfied", customerVariant.frames_satisfied, TimeSpan.FromMilliseconds(100), false);
        spriteObj.LoadAnim("cus_fail", customerVariant.frames_fail, TimeSpan.FromMilliseconds(100), false);
        spriteObj.LoadAnim("cus_extra", customerVariant.frames_extra, TimeSpan.FromMilliseconds(100), false);
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

    public void WaitForOrder()
    {
        drinkWanted.StartOrder();
    }

    public void ProcessOrder(JudgementResult result)
    {
        bool isEmpty = orderbox.RemoveInstruction(result);
        if (isEmpty) { EndOrder(); }

        drinkWanted.SuccessInput();
    }

    public void TwinheadifyOrder() { orderbox.TwinHeadMode(); }

    private void EndOrder()
    {
        orderFulfilled?.Invoke();
        orderFulfilled = null;

        drinkWanted.EndOrder();

        spriteObj.tweener.TweenTo(spriteObj, player => player.position, endPos, 1f)
        .Easing(EasingFunctions.CubicIn).OnEnd(_ => Free());

        spriteObj.animatedSprite.SetAnimation("cus_satisfied");
    }

    private void Free()
    {
        spriteObj.Free();
        drinkWanted.DestroyWantedDrink();
    }

    public void visual_offsetBoxLayer(int n)
    {
        orderbox.order_box.layer += n;
    }
}