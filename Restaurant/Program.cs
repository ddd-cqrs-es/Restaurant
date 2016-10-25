using Restaurant.Helpers;
using Restaurant.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Infrastructure;
using Restaurant.Workers.Abstract;

namespace Restaurant
{
    internal class Program
    {
        private static void Main()
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            stopWatch.Start();
            
            Console.WriteLine("Starting order...");

            var cashier = new Cashier(new OrderPrinter());
            var assistantManager = new AssistantManager(cashier);
            
            var waiter = new Waiter(
                new RoundRobinDispatch(
                    new IOrderHandler[]
                    {
                        new QueuedHandler(new Cook("Bogdan", assistantManager)),
                        new QueuedHandler(new Cook("Roman", assistantManager)),
                        new QueuedHandler(new Cook("Waclaw", assistantManager))
                    }));
            
            waiter.PlaceOrder(1, new List<string> { "pizza", "pasta", "beer", "wine" });
            waiter.PlaceOrder(2, new List<string> { "beer", "beer", "beer", "beer" });
            waiter.PlaceOrder(3, new List<string> { "pizza", "pizza", "pizza", "pizza", "pasta", "pasta", "pasta", "pasta", "wine", "wine", "wine", "wine", "wine", "wine" });

            var unpaidOrders = cashier.GetOutstandingOrders().ToList();
            foreach (var orderId in unpaidOrders)
            {
                Console.WriteLine($"paying for {orderId}");
                cashier.Pay(orderId);
            }

            Console.WriteLine($"Processing {unpaidOrders.Count} orders took: {TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds)}");
            stopWatch.Stop();
            Console.ReadKey();
        }
    }
}
