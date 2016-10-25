using System.Collections.Generic;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;
using Restaurant.Workers;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class TopicBasedPubSub : IPublisher // instead of IOrderHandler
    {
        private Dictionary<string, List<dynamic>> _subscribers = new Dictionary<string, List<dynamic>>();
        private readonly object _lock = new object();

        public void Publish<T>(T message)
        {
            foreach (var subscriber in _subscribers[typeof(T).ToString()])
            {
                subscriber.Handle(message);
            }
        }

        // publish<T>(T m)
        // subscribe<T>(Handles<T> h)

        public void Subscribe<T>(IHandler<T> subscriber)
        {
            lock (_lock)
            {
                var subscribers = 
                    new Dictionary<string, List<dynamic>>(_subscribers);

                if (subscribers.ContainsKey(typeof(T).ToString()))
                {
                    subscribers[typeof(T).ToString()].Add(subscriber);
                }
                else
                {
                    subscribers.Add(
                        typeof(T).ToString(),
                        new List<dynamic>
                        {
                            subscriber
                        });
                }

                _subscribers = subscribers;
            }
        }
    }
}