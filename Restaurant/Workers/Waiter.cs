using System;
using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Events;
using Restaurant.Infrastructure.Abstract;

namespace Restaurant.Workers
{
    public class Waiter
    {
        private readonly IPublisher _orderPublisher;
        private readonly Dictionary<string, decimal> _menu = new Dictionary<string, decimal>
        {
            {"pizza", 9m },
            {"pasta", 11m },
            {"beer", 5m },
            {"wine", 7m }
        };

        public Waiter(IPublisher orderPublisher)
        {
            _orderPublisher = orderPublisher;
        }

        public async void PlaceOrder(int tableNumber, List<string> items)
        {
            await Task.Run(
                () =>
                {
                    try
                    {
                        var i = items
                            .GroupBy(item => item)
                            .Select(
                                itemGroup => new OrderItem()
                                {
                                    Description = itemGroup.Key,
                                    Price = _menu[itemGroup.Key],
                                    Quantity = items.Count(item => item == itemGroup.Key)
                                })
                            .ToList();
                        var order = new Order
                        {
                            TableNumber = tableNumber,
                            Items = i
                        };
                        _orderPublisher.Publish(new OrderPlaced(order));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    
                });
        }
    }
}
