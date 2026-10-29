 using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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

    private bool altDrink = false;

    public Customer(Texture2D _texture2D, CustomerVariantData customerVariant)
    {
        spriteObj = new SpriteObject("customer", _texture2D, customerVariant.texture_region, startPos);
        spriteObj.anchor = new Vector2(0.5f, 1f);
        spriteObj.layer = 1;

        altDrink = Random.Shared.Next() % 2 == 0;
        drinkWanted.CreateDrinkSprite(altDrink);
        orderbox.CreateOrderBoxSprite();

        spriteObj.LoadAnim("cus_talk", customerVariant.frames_talk, TimeSpan.FromMilliseconds(100), false);
        spriteObj.LoadAnim("cus_talk_alt", customerVariant.frames_talk_alt, TimeSpan.FromMilliseconds(100), false);
        spriteObj.LoadAnim("cus_satisfied", customerVariant.frames_satisfied, TimeSpan.FromMilliseconds(100), false);
        spriteObj.LoadAnim("cus_satisfied_alt", customerVariant.frames_satisfied_alt, TimeSpan.FromMilliseconds(100), false);
        spriteObj.LoadAnim("cus_fail", customerVariant.frames_fail, TimeSpan.FromMilliseconds(100), false);
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
        spriteObj.animatedSprite.SetAnimation(twinHeadMode ? "cus_talk_alt" : "cus_talk");
        orderbox.InstructionAdded(input);
    }

    public void WaitForOrder()
    {
        drinkWanted.StartOrder();
    }

    private bool isPerfect = true;
    public void ProcessOrder(JudgementResult result)
    {
        bool isEmpty = orderbox.RemoveInstruction(result);
        if (isEmpty) { EndOrder(); }

        if (result == JudgementResult.miss)
        {
            isPerfect = false;
            spriteObj.animatedSprite.SetAnimation("cus_fail");
            drinkWanted.MessUp();
            return;
        }
        drinkWanted.SuccessInput();
    }

    private bool twinHeadMode = false;
    public void TwinheadifyOrder() { orderbox.TwinHeadMode(); twinHeadMode = true; }

    private void EndOrder()
    {
        orderFulfilled?.Invoke();
        orderFulfilled = null;

        drinkWanted.EndOrder();

        spriteObj.tweener.TweenTo(spriteObj, player => player.position, endPos, 1f)
        .Easing(EasingFunctions.CubicIn).OnEnd(_ => Free());

        if (isPerfect)
        {
            spriteObj.animatedSprite.SetAnimation(altDrink ? "cus_satisfied_alt" : "cus_satisfied");
        }
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