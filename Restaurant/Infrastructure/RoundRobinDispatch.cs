using System.Collections.Generic;
using System.Linq;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class RoundRobinDispatch : IOrderHandler
    {
        private IEnumerable<IOrderHandler> _orderHandlers;

        public RoundRobinDispatch(IEnumerable<IOrderHandler> orderHandlers)
        {
            _orderHandlers = orderHandlers;
        }

        public void HandleOrder(Order order)
        {
            var orderHandler = _orderHandlers.First();

            try
            {
                orderHandler.HandleOrder(order);
            }
            finally
            {
                _orderHandlers = _orderHandlers.Skip(1);
            }
        }
    }
}