using System.Collections.Generic;
using Restaurant.Models;
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

        public void Handle(T orderPaid)
        {
            var orderHandler = _queue.Dequeue();

            try
            {
                orderHandler.Handle(orderPaid);
            }
            finally
            {
                _queue.Enqueue(orderHandler);
            }
        }
    }
}