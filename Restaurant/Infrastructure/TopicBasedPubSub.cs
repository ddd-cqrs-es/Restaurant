using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class TopicBasedPubSub : IPublisher 
    {
        private readonly Dictionary<string, List<dynamic>> _subscribers = new Dictionary<string, List<dynamic>>();
        private readonly object _lock = new object();

        public void Publish<T>(T message) where T : Message
        {
            Publish(typeof(T).ToString(), message);
            Publish(message.CorrelationId, message);
        }

        private void Publish<T>(string topic, T message) 
        {
            if (_subscribers.ContainsKey(topic))
            {
                foreach (var subscriber in _subscribers[topic])
                {
                    subscriber.Handle(message);
                }
            }
        }

        public void Subscribe<T>(IHandler<T> subscriber)
        {
            SubscribeByTopic(typeof(T).ToString(), subscriber);
        }

        public void SubscribeByTopic<T>(string topic, IHandler<T> subscriber)
        {
            lock (_lock)
            {

                var key = topic;

                if (_subscribers.ContainsKey(key))
                {
                    var newSubsList = new List<dynamic>(_subscribers[key]);
                    _subscribers[key].Add(subscriber);
                }
                else
                {
                    _subscribers.Add(
                       key,
                        new List<dynamic>
                        {
                            subscriber
                        });
                }
            }
        }
    }
}