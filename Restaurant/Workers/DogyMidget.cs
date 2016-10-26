using System;
using System.Runtime.InteropServices;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Workers.Abstract;

namespace Restaurant.Workers
{
    public class DogyMidget : IMidget
    {
        public Action<string> CleanUp { get; set; }
        private readonly IPublisher _publisher;

        public DogyMidget(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(Message message)
        {
            if (message is OrderPlaced)
            {
                _publisher.Publish(new PriceOrdered(((OrderPlaced)message).Order, message.MessageId));
            }

            if (message is OrderCooked)
            {
                CleanUp(message.CorrelationId);
            }

            if (message is OrderPriced)
            {
                _publisher.Publish(new TakePayment(((OrderPriced)message).Order, message.MessageId));
            }

            if (message is OrderPaid)
            {
                _publisher.Publish(new CookFood(((OrderPaid)message).Order, message.MessageId));
            }
        }
    }
}