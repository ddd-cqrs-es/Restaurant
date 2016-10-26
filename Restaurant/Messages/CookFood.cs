using Restaurant.Models;

namespace Restaurant.Messages
{
    public class CookFood : Message
    {
        public CookFood(Order order, string causationId) :
            base(order.ShoulBeProcessesdBefore, order.Id, causationId)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }
}