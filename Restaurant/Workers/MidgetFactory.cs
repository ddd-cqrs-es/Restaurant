using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;

namespace Restaurant.Workers
{
    public static class MidgetFactory
    {
        public static IMidget CreateMidget(Order order, IPublisher publisher)
        {
            if (order.IsDodgy)
                return new DogyMidget(publisher);

            return new RegularMidget(publisher);
        }
    }
}
