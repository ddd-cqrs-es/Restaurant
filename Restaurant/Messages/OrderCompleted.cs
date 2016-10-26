using Restaurant.Models;

namespace Restaurant.Messages
{
    public class OrderCompleted : Message
    {
        public OrderCompleted(Order order, string causationId) :
            base(order.ShoulBeProcessesdBefore, order.Id, causationId)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }
}