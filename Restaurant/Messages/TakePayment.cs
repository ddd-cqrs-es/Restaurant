using Restaurant.Models;

namespace Restaurant.Messages
{
    public class TakePayment : Message
    {
        public TakePayment(Order order, string causationId) :
            base(order.ShoulBeProcessesdBefore, order.Id, causationId)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }
}