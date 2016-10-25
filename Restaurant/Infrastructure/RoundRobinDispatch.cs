using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class RoundRobinDispatch : IOrderHandler
    {
        private readonly Queue<IOrderHandler> _queue;

        public RoundRobinDispatch(IEnumerable<IOrderHandler> orderHandlers)
        {
            _queue = new Queue<IOrderHandler>(orderHandlers);
        }

        public void HandleOrder(Order order)
        {
            var orderHandler = _queue.Dequeue();

            try
            {
                orderHandler.HandleOrder(order);
            }
            finally
            {
                _queue.Enqueue(orderHandler);
            }
        }
    }
}