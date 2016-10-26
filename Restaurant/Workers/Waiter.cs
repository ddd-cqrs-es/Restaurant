using System;
using Restaurant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;

namespace Restaurant.Workers
{
    public class Waiter
    {
        private readonly IPublisher _orderPublisher;

        private readonly Dictionary<string, decimal> _menu = new Dictionary<string, decimal>
        {
            { "pizza", 9m },
            { "pasta", 11m },
            { "beer", 5m },
            { "wine", 7m }
        };

        public Waiter(IPublisher orderPublisher)
        {
            _orderPublisher = orderPublisher;
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
                    _orderPublisher.Publish(new OrderPlaced(order, Guid.Empty.ToString()));
                });

            return order.Id;
        }
    }
}
