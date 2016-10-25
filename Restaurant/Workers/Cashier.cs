using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Restaurant.Events;
using Restaurant.Infrastructure.Abstract;

namespace Restaurant.Workers
{
    public class Cashier : IHandler<OrderPriced>
    {
        private readonly ConcurrentDictionary<string, OrderPriced> _outstandingOrders = new ConcurrentDictionary<string, OrderPriced>();
        private readonly IPublisher _orderPublisher;

        public Cashier(IPublisher orderPublisher)
        {
            _orderPublisher = orderPublisher;
        }
        public void Handle(OrderPriced orderPaid) 
        {
            _outstandingOrders.TryAdd(Guid.NewGuid().ToString(), orderPaid);
        }

        public void Pay(string orderId)
        {
            Thread.Sleep(100);

            var orderPriced = _outstandingOrders[orderId];
            orderPriced.Order.Paid = true;

            OrderPriced removedOrder;
            _outstandingOrders.TryRemove(orderId, out removedOrder);
            _orderPublisher.Publish(new OrderPaid(removedOrder.Order));
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
