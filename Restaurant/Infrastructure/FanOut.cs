using System.Collections.Generic;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class FanOut<T> : IHandler<T>
    {
        private readonly IEnumerable<IHandler<T>> _orderHandlers;

        public FanOut(IEnumerable<IHandler<T>> orderHandlers)
        {
            _orderHandlers = orderHandlers;
        }

        public void Handle(T orderCooked)
        {
            foreach (var orderHandler in _orderHandlers)
            {
                orderHandler.Handle(orderCooked);
            }
        }
    }
}