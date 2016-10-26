using System.Collections.Generic;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Workers.Abstract;

namespace Restaurant.Workers
{
    public class MidgetHouse : IHandler<OrderPlaced>, IHandler<Message>
    {
        private readonly IPublisher _publisher;
        private readonly Dictionary<string, IMidget> _midgets = new Dictionary<string, IMidget>();

        public MidgetHouse(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public QueuedHandler<Message> QueuedHandler { get; set; }  

        public void Handle(OrderPlaced orderPlaced)
        {
            var midget = MidgetFactory.CreateMidget(orderPlaced.Order, _publisher);
            midget.CleanUp = corId => _midgets.Remove(corId);
            _midgets.Add(orderPlaced.CorrelationId, midget);

            _publisher.SubscribeByTopic(orderPlaced.CorrelationId, QueuedHandler);
        }

        public void Handle(Message message)
        {
            _midgets[message.CorrelationId].Handle(message);
        }
    }
}