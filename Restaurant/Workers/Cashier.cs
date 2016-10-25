using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Restaurant.Workers
{
    public class Cashier : IOrderHandler
    {
        private readonly ConcurrentDictionary<string, Order> _outstandingOrders = new ConcurrentDictionary<string, Order>();
        private readonly IPublisher _orderPublisher;

        public Cashier(IPublisher orderPublisher)
        {
            _orderPublisher = orderPublisher;
        }
        public void HandleOrder(Order order)
        {
            _outstandingOrders.TryAdd(Guid.NewGuid().ToString(), order);
        }

        public void Pay(string orderId)
        {
            Thread.Sleep(100);

            var order = _outstandingOrders[orderId];
            order.Paid = true;

            Order removedOrder;
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
