using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class QueuedHandler<T> : IHandler<T>, IStartable
    {
        private readonly ConcurrentQueue<T> _queue;

        private readonly IHandler<T> _handler;

        public string Name { get; }

        public int QueueLength => _queue.Count;

        public QueuedHandler(string name, IHandler<T> handler)
        {
            _queue = new ConcurrentQueue<T>();
            _handler = handler;

            Name = name;
        }

        public void Handle(T orderPlaced)
        {
            _queue.Enqueue(orderPlaced);
        }

        public async void Start()
        {
            await Task.Run(
                () =>
                {
                    while (true)
                    {
                        T message;
                        var dequeueSuccuded = _queue.TryDequeue(out message);
                        if (!dequeueSuccuded)
                        {
                            Thread.Sleep(100);
                        }
                        else
                        {
                            _handler.Handle(message);
                        }
                    }
                });
        }
    }
}