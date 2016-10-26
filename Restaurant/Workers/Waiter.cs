using System;
using Restaurant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Workers.Abstract;

namespace Restaurant.Workers
{
    public class MidgetHouse : IHandler<OrderPlaced>, IHandler<Message>
    {
        private readonly IPublisher _publisher;
        private readonly Dictionary<string, Midget> _midgets = new Dictionary<string, Midget>();

        public MidgetHouse(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public QueuedHandler<Message> QueuedHandler { get; set; }  

        public void Handle(OrderPlaced orderPlaced)
        {
            var midget = new Midget(_publisher)
            {
                CleanUp = corId => _midgets.Remove(corId)
            };
            _midgets.Add(orderPlaced.CorrelationId, midget);

            _publisher.SubscribeByTopic(orderPlaced.CorrelationId, QueuedHandler);
        }

        public void Handle(Message message)
        {
            _midgets[message.CorrelationId].Handle(message);
        }
    }

    public class Waiter
    {
        private readonly IPublisher _publisher;

        private readonly Dictionary<string, decimal> _menu = new Dictionary<string, decimal>
        {
            { "pizza", 9m },
            { "pasta", 11m },
            { "beer", 5m },
            { "wine", 7m }
        };

        public Waiter(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public string PlaceOrder(int tableNumber, List<string> items)
        {
            var order = new Order
            {
                TableNumber = tableNumber,
                Items = items
                            .GroupBy(item => item)
                            .Select(
                                itemGroup => new OrderItem
                                {
                                    Description = itemGroup.Key,
                                    Price = _menu[itemGroup.Key],
                                    Quantity = items.Count(item => item == itemGroup.Key)
                                })
                            .ToList()
            };

            Task.Run(
                () =>
                {
                    _publisher.Publish(new OrderPlaced(order, Guid.Empty.ToString()));
                });

            return order.Id;
        }
    }
}
