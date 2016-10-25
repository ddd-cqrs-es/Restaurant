using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Restaurant.Workers
{
    public class AssistantManager : IOrderHandler
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

        public void HandleOrder(Order order) 
        {
            Thread.Sleep(100);

            order.Tax = order.Items.Sum(item => _calculationRules[item.Description] * item.Quantity);
            order.Total = order.Items.Sum(item => item.Price * item.Quantity);

            _orderPublisher.Publish(Topics.BillProduced, order);
        }
    }
}
