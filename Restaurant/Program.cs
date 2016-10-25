using Restaurant.Helpers;
using Restaurant.Workers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            stopWatch.Start();

            Console.WriteLine("Starting order...\n");

            var cashier = new Cashier(new OrderPrinter());
            var waiter = new Waiter(new Cook(new AssistantManager(cashier)));

            waiter.PlaceOrder(1, new List<string> { "pizza", "pasta", "beer", "wine" });
            waiter.PlaceOrder(2, new List<string> { "beer", "beer", "beer", "beer" });
            waiter.PlaceOrder(3, new List<string> { "pizza", "pizza", "pizza", "pizza", "pasta", "pasta", "pasta", "pasta", "wine", "wine", "wine", "wine", "wine", "wine" });

            var unpaidOrders = cashier.GetOutstandingOrders();
            foreach (var orderId in unpaidOrders)
            {
                Console.WriteLine("paying for {0}\n", orderId);
                cashier.Pay(orderId);
            }

            Console.WriteLine($"Processing {unpaidOrders.ToList().Count} orders took: {TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds)}");
            stopWatch.Stop();
            Console.ReadKey();
        }
    }
}
