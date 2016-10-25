using System.Collections.Generic;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;
using Restaurant.Workers;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class TopicBasedPubSub : IPublisher // instead of IOrderHandler
    {
        private Dictionary<Topics, List<IHandler<>>> _subscribers = new Dictionary<Topics, List<IHandler<>>>();
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

        public void Subscribe(Topics topic, IHandler<> subscriber)
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
                        new List<IHandler<>>
                        {
                            subscriber
                        });
                }

                _subscribers = subscribers;
            }
        }
    }
}