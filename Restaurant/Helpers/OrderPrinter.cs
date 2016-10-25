using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System;

namespace Restaurant.Helpers
{
    public class OrderPrinter : IOrderHandler
    {
        public void HandleOrder(Order order)
        {
            //Console.WriteLine(order.ToJsonString());
        }
    }
}
