using Restaurant.Helpers;
using Restaurant.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;

namespace Restaurant
{
    internal class Program
    {
        private static void Main()
        {
            var publisher = new TopicBasedPubSub();

            var cashier = new Cashier(publisher);
            var cashierQueue = new QueuedHandler<TakePayment>("Cashier", cashier);
            var assistantManager = new QueuedHandler<PriceOrdered>("AssistantManager", new AssistantManager(publisher));

            var seed = new Random(DateTime.Now.Millisecond);

            var cook1 = new QueuedHandler<CookFood>(
                "BogdanQueue",
                new TTLHandler<CookFood>(new Cook(seed.Next(1000), publisher)));
            var cook2 = new QueuedHandler<CookFood>(
                "RomanQueue",
                new TTLHandler<CookFood>(new Cook(seed.Next(1000), publisher)));
            var cook3 = new QueuedHandler<CookFood>(
                "WaclawQueue",
                new TTLHandler<CookFood>(new Cook(seed.Next(1000), publisher)));

            var dispatcher = new QueuedHandler<CookFood>(
                "MFDispatcher",
                new TTLHandler<CookFood>(
                    new MFDispatcher<CookFood>(
                        new List<QueuedHandler<CookFood>>
                        {
                            cook1,
                            cook2,
                            cook3
                        })));

            StartQueues(
                new List<IStartable>
                {
                    assistantManager,
                    cashierQueue,
                    dispatcher,
                    cook1,
                    cook2,
                    cook3
                });
            StartQueuePrinter(
                new List<IPrintable>
                {
                    assistantManager,
                    cashierQueue,
                    dispatcher,
                    cook1,
                    cook2,
                    cook3
                });

            var waiter = new Waiter(publisher);

            
            publisher.Subscribe(dispatcher);
            publisher.Subscribe(assistantManager);
            publisher.Subscribe(cashier);
            
            PlaceOrders(waiter, publisher);

            HandlePays(cashier);

            Console.ReadKey();
        }

        private static void StartQueues(IEnumerable<IStartable> queues)
        {
            foreach (var queue in queues)
            {
                queue.Start();
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

        private static void StartQueuePrinter(List<IPrintable> queues)
        {
            Task.Run(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        foreach (var queue in queues)
                        {
                            queue.Print();
                        }
                    }
                });
        }

        private static void PlaceOrders(Waiter waiter, TopicBasedPubSub publisher)
        {
            for (var i = 0; i < 200; i++)
            {
                var correlationId = waiter.PlaceOrder(
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

                publisher.SubscribeByTopic(correlationId, new Printer());
            }
        }
    }
}
