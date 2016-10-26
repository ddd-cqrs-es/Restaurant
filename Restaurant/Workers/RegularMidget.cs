using System;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Workers.Abstract;

namespace Restaurant.Workers
{
    public class RegularMidget : IMidget
    {
        public Action<string> CleanUp { get; set; }
        private readonly IPublisher _publisher;

        public RegularMidget(IPublisher publisher)
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
}