using System.Collections.Concurrent;
using System.Threading;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class QueuedHandler : IOrderHandler, IStartable
    {
        private readonly ConcurrentQueue<Order> _queue;

        private readonly IOrderHandler _orderHandler;

        public QueuedHandler(IOrderHandler orderHandler)
        {
            _queue = new ConcurrentQueue<Order>();
            _orderHandler = orderHandler;
        }

        public void HandleOrder(Order order)
        {
            _queue.Enqueue(order);
        }

        public void Start()
        {
            while (true)
            {
                Order order;
                var dequeueSuccuded = _queue.TryDequeue(out order);
                if (!dequeueSuccuded)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    _orderHandler.HandleOrder(order);
                }
            }
        }
    }
}