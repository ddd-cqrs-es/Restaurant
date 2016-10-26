using Restaurant.Models;

namespace Restaurant.Messages
{
    public class OrderPaid : Message
    {
        public Order Order { get; }
        
        public OrderPaid(Order order, string causationId) :
            base(order.ShoulBeProcessesdBefore, order.Id, causationId)
        {
            Order = order;
        }
    }
}