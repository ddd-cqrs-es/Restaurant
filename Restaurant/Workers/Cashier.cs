using Restaurant.Workers.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;

namespace Restaurant.Workers
{
    
    public class Midget : IHandler<Message>
    {
        public Action<string> CleanUp;
        private readonly IPublisher _publisher;

        public Midget(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(Message message)
        {
            if (message is OrderPlaced)
            {
                _publisher.Publish(new CookFood(((OrderPlaced)message).Order, message.MessageId));
            }

            if (message is OrderCooked)
            {
                _publisher.Publish(new PriceOrdered(((OrderCooked)message).Order, message.MessageId));
            }

            if (message is OrderPriced)
            {
                _publisher.Publish(new TakePayment(((OrderPriced)message).Order, message.MessageId));
            }

            if (message is OrderPaid)
            {
                CleanUp(message.CorrelationId);
            }
        }
    }

    public class Cashier : IHandler<TakePayment>
    {
        private readonly ConcurrentDictionary<string, TakePayment> _outstandingOrders =
            new ConcurrentDictionary<string, TakePayment>();
        private readonly IPublisher _publisher;

        public Cashier(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(TakePayment message)
        {
            _outstandingOrders.TryAdd(Guid.NewGuid().ToString(), message);
        }

        public void Pay(string orderId)
        {
            var orderPriced = _outstandingOrders[orderId];
            orderPriced.Order.Paid = true;

            TakePayment removedOrder;
            _outstandingOrders.TryRemove(orderId, out removedOrder);
            _publisher.Publish(new OrderPaid(removedOrder.Order, removedOrder.MessageId));
        }

        public IEnumerable<string> GetOutstandingOrders()
        {
            return _outstandingOrders.Keys.Select(x => x).ToList();
        }
    }
}
