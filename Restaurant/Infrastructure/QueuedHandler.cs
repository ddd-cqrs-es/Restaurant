using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class QueuedHandler : IOrderHandler, IStartable
    {
        private readonly ConcurrentQueue<Order> _queue;

        private readonly IOrderHandler _orderHandler;

        public string Name { get; }

        public int QueueLength => _queue.Count;

        public QueuedHandler(string name, IOrderHandler orderHandler)
        {
            _queue = new ConcurrentQueue<Order>();
            _orderHandler = orderHandler;

            Name = name;
        }

        public void HandleOrder(Order order)
        {
            _queue.Enqueue(order);
        }

        public async void Start()
        {
            await Task.Run(
                () =>
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
                });
        }
    }
}