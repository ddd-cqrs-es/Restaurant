using Restaurant.Helpers;
using Restaurant.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant
{
    // cook -> handles<T> -> void handle(T)

    public class TopicBasedPubSub : IPublisher // instead of IOrderHandler
    {
        private Dictionary<Topics, List<IOrderHandler>> _subscribers = new Dictionary<Topics, List<IOrderHandler>>();
        private readonly object _lock = new object();

        public void Publish(Topics topic, Order message)
        {
            foreach (var subscriber in _subscribers[topic])
            {
                subscriber.HandleOrder(message);
            }
        }

        // publish<T>(T m)
        // subscribe<T>(Handles<T> h)

        public void Subscribe(Topics topic, IOrderHandler subscriber)
        {
            lock (_lock)
            {
                var subscribers = _subscribers;

                if (subscribers.ContainsKey(topic))
                {
                    subscribers[topic].Add(subscriber);
                }
                else
                {
                    subscribers.Add(
                        topic,
                        new List<IOrderHandler>
                        {
                            subscriber
                        });
                }

                _subscribers = subscribers;
            }
        }
    }

    public interface IPublisher
    {
        void Publish(Topics topic, Order message);
    }

    internal class Program
    {
        private static void Main()
        {
            var publisher = new TopicBasedPubSub();

            var cashier = new Cashier(publisher);
            var cashierQueue = new QueuedHandler("Cashier", cashier);
            var assistantManager = new QueuedHandler("AssistantManager", new AssistantManager(publisher));
            var cooks = GetCooks(publisher);
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
            
            var waiter = new Waiter(publisher);

            publisher.Subscribe(Topics.OrderReceived, dispatcher);
            publisher.Subscribe(Topics.FoodCooked, assistantManager);
            publisher.Subscribe(Topics.BillProduced, cashier);
            publisher.Subscribe(Topics.OrderPaid, new OrderPrinter());

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

        private static List<QueuedHandler> GetCooks(IPublisher publisher)
        {
            var seed = new Random(DateTime.Now.Millisecond);

            var cooks = new List<QueuedHandler>
            {
                new QueuedHandler("Bogdan", new TTLHandler(new Cook(seed.Next(1000), "Bogdan", publisher))),
                new QueuedHandler("Roman", new TTLHandler(new Cook(seed.Next(1000), "Roman", publisher))),
                new QueuedHandler("Waclaw", new TTLHandler(new Cook(seed.Next(1000), "Waclaw", publisher)))
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
