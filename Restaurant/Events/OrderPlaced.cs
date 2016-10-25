using Restaurant.Models;

namespace Restaurant.Events
{
    public class OrderPlaced
    {
        public Order Order { get; set; }

        public OrderPlaced(Order order)
        {
            Order = order;
        }
    }
}