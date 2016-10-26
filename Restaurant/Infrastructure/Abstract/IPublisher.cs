using Restaurant.Messages;

namespace Restaurant.Infrastructure.Abstract
{
    public interface IPublisher
    {
        void Publish<T>(T message) where T : Message;
    }
}