using Restaurant.Models;

namespace Restaurant.Messages
{
    public class OrderPriced : Message
    {
        public Order Order { get; }

        public OrderPriced(Order order, string causationId) : 
            base(order.ShoulBeProcessesdBefore, order.Id, causationId)
        {
            Order = order;
        }
    }
}