using Restaurant.Workers.Abstract;
using System;
using Restaurant.Events;

namespace Restaurant.Helpers
{
    public class Printer : IHandler<OrderPaid>
    {
        public void Handle(OrderPaid orderPaid)
        {
            Console.WriteLine(orderPaid.Order.ToJsonString());
        }
    }
}
