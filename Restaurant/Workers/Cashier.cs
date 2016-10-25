using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Restaurant.Infrastructure.Abstract;

namespace Restaurant.Workers
{
    public class Cashier<T> : IHandler<T> where T : Order
    {
        private readonly ConcurrentDictionary<string, T> _outstandingOrders = new ConcurrentDictionary<string, T>();
        private readonly IPublisher _orderPublisher;

        public Cashier(IPublisher orderPublisher)
        {
            _orderPublisher = orderPublisher;
        }
        public void Handle(T order) 
        {
            _outstandingOrders.TryAdd(Guid.NewGuid().ToString(), order);
        }

        public void Pay(string orderId)
        {
            Thread.Sleep(100);

            var order = _outstandingOrders[orderId];
            order.Paid = true;

            T removedOrder;
            _outstandingOrders.TryRemove(orderId, out removedOrder);
            _orderPublisher.Publish(Topics.OrderPaid, order);
        }

        public IEnumerable<string> GetOutstandingOrders()
        {
            try
            {
                return _outstandingOrders.Keys.Select(x => x).ToList();
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e);
                return new List<string>();
            }
        }
    }
}
