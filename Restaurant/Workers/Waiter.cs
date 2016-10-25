using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Restaurant.Workers
{
    public class Waiter
    {
        private IOrderHandler _orderHandler;
        private Dictionary<string, decimal> _menu = new Dictionary<string, decimal>
        {
            {"pizza", 9m },
            {"pasta", 11m },
            {"beer", 5m },
            {"wine", 7m }
        };

        public Waiter(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }

        public void PlaceOrder(int tableNumber, List<string> items)
        {
            Thread.Sleep(100);

            var order = new Order();
            order.TableNumber = tableNumber;

            order.Items = items
                .GroupBy(item => item)
                .Select(itemGroup => new OrderItem()
                {
                    Description = itemGroup.Key,
                    Price = _menu[itemGroup.Key],
                    Quantity = items.Count(item => item == itemGroup.Key)
                })
                .ToList();
            
            _orderHandler.HandleOrder(order);
        }
    }
}
