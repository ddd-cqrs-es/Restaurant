using System;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class TTLHandler : IOrderHandler
    {
        private readonly IOrderHandler _orderHandler;

        public TTLHandler(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }

        public void HandleOrder(Order order)
        {
            if (order.ShoulBeProcessesdBefore > DateTime.Now.Add(-TimeSpan.FromMinutes(0.5)))
            {
                _orderHandler.HandleOrder(order);
            }
            else
            {
                Console.WriteLine($"skipping {order.TableNumber}");
            }
        }
    }
}