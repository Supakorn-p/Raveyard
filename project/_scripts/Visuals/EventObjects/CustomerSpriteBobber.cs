using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Raveyard;

public class CustomerSpriteBobber : EventObject
{
    private double beatDuration = 1;
    private float distance = 0;

    private CustomerQueue customerQueue;
    private RecordPlayer recordPlayer;

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "customer_bobbing";
    }

    public void InjectCustomerQueue(CustomerQueue _customerQueue) {customerQueue = _customerQueue;}
    public void InjectRecordPlayer(RecordPlayer _recordPlayer) {recordPlayer = _recordPlayer;}

    public override void Update(GameTime gameTime)
    {
        if (customerQueue == null || recordPlayer == null) { return; }

        Customer currentCustomer = customerQueue.getFirstCustomer();
        if (currentCustomer == null) { return; }

        SpriteObject customerSpr = currentCustomer.spriteObj;

        float offsetY = (float) Math.Sin(recordPlayer.getCurrentBeattime() / beatDuration * Math.PI);
        float absOffsetY = MathF.Abs(offsetY);
        float trueOffsetY = (float) Math.Pow(1.0f - absOffsetY, 2) * distance;

        customerSpr.position_offset = new Vector2(0, trueOffsetY);
    }

    public override void OnEvent(string[] eventParams)
    {
        float newDistance = float.TryParse(eventParams[0], out float result) ? result : distance;
        distance = newDistance;
        
        if (eventParams.GetLength(0) < 2) { return; }

        double newBeatDuration = double.TryParse(eventParams[1], out double result_) ? result_ : beatDuration;
        beatDuration = Math.Max(newBeatDuration, 0.01);
    }
}