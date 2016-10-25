using Restaurant.Models;

namespace Restaurant.Events
{
    public class OrderPaid : Event
    {
        public Order Order { get; set; }

        public OrderPaid(Order order)
        {
            Order = order;
        }
    }
}