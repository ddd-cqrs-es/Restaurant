using System;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;

namespace Restaurant.Workers
{
    public class RegularMidget : IMidget
    {
        private readonly List<Message> _lastMessages = new List<Message>();

        public Action<string> CleanUp { get; set; }
        private readonly IPublisher _publisher;
        private bool _isCooked;

        public RegularMidget(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(Message message)
        {
            if (IsDuplicated(message))
            {
                _publisher.Publish(new DuplicateOrder(DateTime.MaxValue, message.CorrelationId, message.MessageId));

                return;
            }

            _lastMessages.Add(message);

            if (message is CookTimeOut)
            {
                
            }

            if (message is OrderPlaced)
            {
                _isCooked = false;
                var msg = new CookFood(((OrderPlaced)message).Order, message.MessageId);

                _publisher.Publish(new FutureMessage(new CookeTimeOut(msg), DateTime.Now.AddMilliseconds(100)));
                _publisher.Publish(msg);
            }

            if (message is OrderCooked)
            {
                _isCooked = true;
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

        private bool IsDuplicated(Message message)
        {
            return _lastMessages.Any(lastMessage => lastMessage.GetType() == message.GetType());
        }
    }

    public class CookTimeOut : Message
    {
        public CookFood Message { get; set; }

        public CookTimeOut(CookFood message) : 
            base(DateTime.MaxValue, message.CorrelationId, message.MessageId)
        {
            Message = message;
        }
    }
}