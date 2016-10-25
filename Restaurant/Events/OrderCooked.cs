using Restaurant.Models;

namespace Restaurant.Events
{
    public class OrderCooked : Event
    {
        public Order Order { get; set; }

        public OrderCooked(Order order)
        {
            Order = order;
        }
    }
}