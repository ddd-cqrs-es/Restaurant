using System.Collections.Generic;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class FanOut : IOrderHandler
    {
        private readonly IEnumerable<IOrderHandler> _orderHandlers;

        public FanOut(IEnumerable<IOrderHandler> orderHandlers)
        {
            _orderHandlers = orderHandlers;
        }

        public void HandleOrder(Order order)
        {
            foreach (var orderHandler in _orderHandlers)
            {
                orderHandler.HandleOrder(order);
            }
        }
    }
}