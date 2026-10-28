using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace Raveyard;

public class Twinhead : EventObject
{
    private CustomerQueue customerQueue;

    public override void Init(ContentManager Content)
    {
        base.Init(Content);
        eventName = "dst_twin";
    }

    public void InjectCustomerQueue(CustomerQueue _customerQueue) {customerQueue = _customerQueue;}

    public override void OnEvent(string[] eventParams)
    {
        customerQueue.getFirstCustomer().TwinheadifyOrder();
    }
}