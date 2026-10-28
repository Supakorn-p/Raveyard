using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace Raveyard;

public struct CustomerVariantData
{
    public string asset_name;
    public Vector2 texture_region;

    public int[] frames_idle = [0];
    public int[] frames_talk = [0];
    public int[] frames_satisfied = [0];
    public int[] frames_fail = [0];
    public int[] frames_extra = [0];

    public CustomerVariantData() {}
}

public class CustomerQueue
{
    // "double ended queue"
    private Deque<Customer> customerQueue = new Deque<Customer>(2);
    private TextureCollection customerTextures;
    private Bag<CustomerVariantData> customerVariants = new Bag<CustomerVariantData>(3);

    // this should ideally be the customer that has an order being made (but hasn't finished yet)
    public Customer getFirstCustomer() { customerQueue.GetFront(out var output); return output; }

    // this should ideally be the customer that is currently making an order (to not interrupt the first guy)
    public Customer getLastCustomer() { customerQueue.GetBack(out var output); return output; }
    
    public CustomerQueue(TextureCollection customerTex)
    {
        customerTextures = customerTex;
    }

    public void InitializeTexture(CustomerVariantData customer)
    {
        customerTextures.LoadTexture(customer.asset_name);
        customerVariants.Add(customer);
    }

    // TODO: replace below function
    public void InitializeTexture(string asset_name, Vector2 region)
    {
        customerTextures.LoadTexture(asset_name);
        customerVariants.Add(new CustomerVariantData{asset_name = asset_name, texture_region = region});
    }

    public Customer customer;
    public int currentVariant = 0;
    public Customer AddCustomerToLine()
    {
        CustomerVariantData customerVariant = customerVariants[currentVariant];
        customer = new Customer(
            customerTextures.GetTexture(customerVariant.asset_name), 
            customerVariant
        );
        customerQueue.AddToBack(customer);

        // hack to account for weird visual glitches regarding layers, at least for the normal use cases
        customer.visual_offsetBoxLayer(customerQueue.Count);

        customer.orderFulfilled += RemoveFirstCustomer;
        return customer;
    }

    private void RemoveFirstCustomer() { customerQueue.RemoveFromFront(); }
}