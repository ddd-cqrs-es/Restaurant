using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Restaurant.Events;
using Restaurant.Infrastructure.Abstract;

namespace Restaurant.Workers
{
    public class AssistantManager : IHandler<OrderPlaced>
    {
        private readonly IPublisher _orderPublisher;
        private readonly Dictionary<string, decimal> _calculationRules = new Dictionary<string, decimal>
        {
            {"pizza", 2m },
            {"pasta", 3m },
            {"beer", 2m },
            {"wine", 1.5m }
        };

        public AssistantManager(IPublisher orderPublisher)
        {
            _orderPublisher = orderPublisher;
        }

        public void Handle(OrderPlaced orderCooked)
        {
            Thread.Sleep(100);

            orderCooked.Order.Tax = orderCooked.Order.Items.Sum(item => _calculationRules[item.Description] * item.Quantity);
            orderCooked.Order.Total = orderCooked.Order.Items.Sum(item => item.Price * item.Quantity);

            _orderPublisher.Publish(orderCooked);
        }
    }
}
