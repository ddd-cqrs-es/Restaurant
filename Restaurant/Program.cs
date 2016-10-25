using Restaurant.Helpers;
using Restaurant.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Abstract;
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
            var cashierQueue = new QueuedHandler("cashier", cashier);
            var assistantManager = new QueuedHandler("AssistantManager", new AssistantManager(cashierQueue));
            var queues = new List<QueuedHandler>
            {
                assistantManager
            };
            Random Seed = new Random(DateTime.Now.Millisecond);

            var cooks = new List<QueuedHandler>
            {
                new QueuedHandler("Bogdan", new Cook(Seed.Next(1000),"Bogdan", assistantManager)),
                new QueuedHandler("Roman", new Cook(Seed.Next(1000), "Roman", assistantManager)),
                new QueuedHandler("Waclaw", new Cook(Seed.Next(1000), "Waclaw", assistantManager))
            };
            queues.AddRange(cooks);
            queues.Add(cashierQueue);

            foreach (var queue in queues)
            {
                ((IStartable)queue).Start();
            }

            Task.Run(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(500);
                        foreach (var queue in queues)
                        {
                            Console.WriteLine($"{queue.Name} - {queue.QueueLength}");
                        }
                    }
                });

            Task.Run(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(10);
                        var unpaidOrders = cashier.GetOutstandingOrders().ToList();
                        foreach (var orderId in unpaidOrders)
                        {
                            Console.WriteLine($"paying for {orderId}");
                            cashier.Pay(orderId);
                        }
                    }
                });

            var waiter = new Waiter(new RoundRobinDispatch(cooks));
            PlaceOrders(waiter);

            //stopWatch.Stop();
            Console.ReadKey();
            //Console.WriteLine(
            //    $"Processing {unpaidOrders.Count} orders took: {TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds)}");

        }

        private static void PlaceOrders(Waiter waiter)
        {
            for (var i = 0; i < 200; i++)
            {
                waiter.PlaceOrder(
               i,
               new List<string>
               {
                        "pizza",
                        "pizza",
                        "pizza",
                        "pizza",
                        "pasta",
                        "pasta",
                        "pasta",
                        "pasta",
                        "wine",
                        "wine",
                        "wine",
                        "wine",
                        "wine",
                        "wine"
               });
            }
            
        }
    }
}
