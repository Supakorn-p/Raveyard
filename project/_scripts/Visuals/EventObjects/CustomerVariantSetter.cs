using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace Raveyard;

public class CustomerVariantSetter : EventObject
{
    private CustomerQueue customerQueue;

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "set_customer";
    }

    public void InjectCustomerQueue(CustomerQueue _customerQueue) {customerQueue = _customerQueue;}

    public override void OnEvent(string[] eventParams)
    {
        // set_customer:0|VARIANT
        int.TryParse(eventParams[0], out int VARIANT);
        customerQueue.currentVariant = VARIANT;
    }
}