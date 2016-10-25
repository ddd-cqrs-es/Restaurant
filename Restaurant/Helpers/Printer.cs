using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System;
using Restaurant.Events;

namespace Restaurant.Helpers
{
    public class Printer : IHandler<OrderCooked>
    {
        public void Handle(OrderCooked orderCooked)
        {
            Console.WriteLine(orderCooked.Order.ToJsonString());
        }
    }
}
