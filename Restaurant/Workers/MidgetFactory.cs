using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
