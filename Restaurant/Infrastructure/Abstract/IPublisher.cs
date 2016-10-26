using Restaurant.Messages;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure.Abstract
{
    public interface IPublisher
    {
        void Publish<T>(T message) where T : Message;
        void Subscribe<T>(IHandler<T> subscriber);
        void SubscribeByTopic<T>(string topic, IHandler<T> subscriber);
    }
}