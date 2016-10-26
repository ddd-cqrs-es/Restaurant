using System;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Models;

namespace Restaurant.Workers
{
    public class RegularMidget : IMidget
    {
        private Message _lastMessage;

        public Action<string> CleanUp { get; set; }
        private readonly IPublisher _publisher;

        public RegularMidget(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(Message message)
        {
            if (_lastMessage != null && message.GetType() == _lastMessage.GetType())
            {
                _publisher.Publish(new DuplicateOrder(DateTime.MaxValue, message.CorrelationId, message.MessageId));

                return;
            }

            _lastMessage = message;

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