using System.Collections.Generic;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class RoundRobinDispatch<T> : IHandler<T>
    {
        private readonly Queue<IHandler<T>> _queue;

        public RoundRobinDispatch(IEnumerable<IHandler<T>> orderHandlers)
        {
            _queue = new Queue<IHandler<T>>(orderHandlers);
        }

        public void Handle(T message)
        {
            var orderHandler = _queue.Dequeue();

            try
            {
                orderHandler.Handle(message);
            }
            finally
            {
                _queue.Enqueue(orderHandler);
            }
        }
    }
}