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

            var alarmClock = new AlarmClock(publisher);
            var midgetHouse = new MidgetHouse(publisher);
            var midgetHouseHandler = new QueuedHandler<Message>("MidgetHouse", midgetHouse);
            midgetHouse.QueuedHandler = midgetHouseHandler;

            var cashier = new Cashier(publisher);
            var cashierQueue = new QueuedHandler<TakePayment>("Cashier", cashier);
            var assistantManager = new QueuedHandler<PriceOrdered>("AssistantManager", new FuzzyHandler<PriceOrdered>(new AssistantManager(publisher), 0, 20));

            var seed = new Random(DateTime.Now.Millisecond);

            var cook1 = new QueuedHandler<CookFood>(
                "BogdanQueue",
                new TTLHandler<CookFood>(new FuzzyHandler<CookFood>(new Cook(seed.Next(1000), publisher), 10, 10)));
            var cook2 = new QueuedHandler<CookFood>(
                "RomanQueue",
                new TTLHandler<CookFood>(new FuzzyHandler<CookFood>(new Cook(seed.Next(1000), publisher), 95, 50)));
            var cook3 = new QueuedHandler<CookFood>(
                "WaclawQueue",
                new TTLHandler<CookFood>(new FuzzyHandler<CookFood>(new Cook(seed.Next(1000), publisher), 10, 20)));

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
                    cook3,
                    midgetHouseHandler,
                    alarmClock
                });
            StartQueuePrinter(
                new List<IPrintable>
                {
                    assistantManager,
                    cashierQueue,
                    dispatcher,
                    cook1,
                    cook2,
                    cook3,
                    midgetHouseHandler
                });

            var waiter = new Waiter(publisher);
            
            publisher.Subscribe(dispatcher);
            publisher.Subscribe(assistantManager);
            publisher.Subscribe(cashier);
            publisher.Subscribe(alarmClock);
            publisher.Subscribe<OrderPlaced>(midgetHouse);

            PlaceOrders(waiter);

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
                },
                i % 2 == 0);
            }
        }
    }
}
