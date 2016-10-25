using Restaurant.Helpers;
using Restaurant.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Events;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;

namespace Restaurant
{
    internal class Program
    {
        private static void Main()
        {
            var publisher = new TopicBasedPubSub();
            
            var cashier = new Cashier(publisher);
            var cashierQueue = new QueuedHandler<OrderPriced>("Cashier", cashier);
            var assistantManager = new QueuedHandler<OrderPlaced>("AssistantManager", new AssistantManager(publisher));

            var seed = new Random(DateTime.Now.Millisecond);

            var cook1 = new QueuedHandler<OrderPlaced>(
                "Bogdan",
                new TTLHandler<OrderPlaced>(new Cook(seed.Next(1000), "Bogdan", publisher)));
            var cook2 = new QueuedHandler<OrderPlaced>(
                "Roman",
                new TTLHandler<OrderPlaced>(new Cook(seed.Next(1000), "Roman", publisher)));
            var cook3 = new QueuedHandler<OrderPlaced>(
                "Waclaw",
                new TTLHandler<OrderPlaced>(new Cook(seed.Next(1000), "Waclaw", publisher)));

            var dispatcher = new QueuedHandler<Order>("MFDispatcher", new TTLHandler<Order>(new MFDispatcher<Order>(new List<QueuedHandler<Order>> { cook1, cook2, cook3 })));

            var queues = new List<QueuedHandler<Order>>
            {
                assistantManager,
                cashierQueue,
                dispatcher,
                cook1,
                cook2,
                cook3
            };

            StartQueues(queues);
            StartQueuePrinter(queues);

            var waiter = new Waiter(publisher);

            publisher.Subscribe(dispatcher);
            publisher.Subscribe(assistantManager);
            publisher.Subscribe(cashier);
            publisher.Subscribe(new Printer());

            PlaceOrders(waiter);

            HandlePays(cashier);

            Console.ReadKey();
        }

        private static void StartQueues<T>(List<QueuedHandler<T>> queues)
        {
            foreach (var queue in queues)
            {
                ((IStartable)queue).Start();
            }
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

        private static void StartQueuePrinter(List<QueuedHandler<Order>> queues)
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
