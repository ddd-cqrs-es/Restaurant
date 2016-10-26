using Restaurant.Workers.Abstract;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;

namespace Restaurant.Workers
{
    public class AssistantManager : IHandler<PriceOrdered>
    {
        private readonly IPublisher _publisher;

        private readonly Dictionary<string, decimal> _calculationRules = new Dictionary<string, decimal>
        {
            { "pizza", 2m },
            { "pasta", 3m },
            { "beer", 2m },
            { "wine", 1.5m }
        };

        public AssistantManager(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(PriceOrdered message)
        {
            message.Order.Tax = message.Order.Items.Sum(item => _calculationRules[item.Description] * item.Quantity);
            message.Order.Total = message.Order.Items.Sum(item => item.Price * item.Quantity);

            _publisher.Publish(new OrderPriced(message.Order, message.MessageId));
        }
    }
}
