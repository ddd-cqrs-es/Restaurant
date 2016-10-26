using Restaurant.Models;

namespace Restaurant.Messages
{
    public class OrderCooked : Message
    {
        public Order Order { get; }
        
        public OrderCooked(Order order, string causationId) :
            base(order.ShoulBeProcessesdBefore, order.Id, causationId)
        {
            Order = order;
        }
    }
}