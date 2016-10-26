using Restaurant.Models;

namespace Restaurant.Messages
{
    public class PriceOrdered : Message
    {
        public PriceOrdered(Order order, string causationId) :
            base(order.ShoulBeProcessesdBefore, order.Id, causationId)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }
}