using Restaurant.Models;

namespace Restaurant.Events
{
    public class OrderCooked
    {
        public Order Order { get; set; }

        public OrderCooked(Order order)
        {
            Order = order;
        }
    }
}