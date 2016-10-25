using Restaurant.Helpers;
using Restaurant.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Abstract;

namespace Restaurant
{
    internal class Program
    {
        private static void Main()
        {
            var cashier = new Cashier(new OrderPrinter());
            var cashierQueue = new QueuedHandler("cashier", cashier);
            var assistantManager = new QueuedHandler("AssistantManager", new AssistantManager(cashierQueue));
            var cooks = GetCooks(assistantManager);
            var dispatcher = new QueuedHandler("MFDispatcher", new TTLHandler(new MFDispatcher(cooks)));

            var queues = new List<QueuedHandler>
            {
                assistantManager,
                cashierQueue,
                dispatcher
            };
            queues.AddRange(cooks);
            
            StartQueues(queues);
            StartQueuePrinter(queues);
            
            var waiter = new Waiter(dispatcher);
            
            PlaceOrders(waiter);

            HandlePays(cashier);

            Console.ReadKey();
        }

        private static void StartQueues(List<QueuedHandler> queues)
        {
            foreach (var queue in queues)
            {
                ((IStartable)queue).Start();
            }
        }

        private static List<QueuedHandler> GetCooks(QueuedHandler assistantManager)
        {
            var seed = new Random(DateTime.Now.Millisecond);

            var cooks = new List<QueuedHandler>
            {
                new QueuedHandler("Bogdan", new TTLHandler(new Cook(seed.Next(1000), "Bogdan", assistantManager))),
                new QueuedHandler("Roman", new TTLHandler(new Cook(seed.Next(1000), "Roman", assistantManager))),
                new QueuedHandler("Waclaw", new TTLHandler(new Cook(seed.Next(1000), "Waclaw", assistantManager)))
            };

            return cooks;
        }

        private static void HandlePays(Cashier cashier)
        {
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
        }

        private static void StartQueuePrinter(List<QueuedHandler> queues)
        {
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
