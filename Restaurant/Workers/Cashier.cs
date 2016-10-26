using Restaurant.Workers.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;

namespace Restaurant.Workers
{
    public class Cashier : IHandler<OrderPriced>
    {
        private readonly ConcurrentDictionary<string, OrderPriced> _outstandingOrders =
            new ConcurrentDictionary<string, OrderPriced>();
        private readonly IPublisher _publisher;

        public Cashier(IPublisher publisher)
        {
            _publisher = publisher;
        }
        public void Handle(OrderPriced message)
        {
            _outstandingOrders.TryAdd(Guid.NewGuid().ToString(), message);
        }

        public void Pay(string orderId)
        {
            var orderPriced = _outstandingOrders[orderId];
            orderPriced.Order.Paid = true;

            OrderPriced removedOrder;
            _outstandingOrders.TryRemove(orderId, out removedOrder);
            _publisher.Publish(new OrderPaid(removedOrder.Order, removedOrder.MessageId));
        }

        public IEnumerable<string> GetOutstandingOrders()
        {
            return _outstandingOrders.Keys.Select(x => x).ToList();
        }
    }
}
