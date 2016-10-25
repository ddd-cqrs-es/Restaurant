namespace Restaurant.Infrastructure.Abstract
{
    public interface IPublisher
    {
        void Publish<T>(T message);
    }
}