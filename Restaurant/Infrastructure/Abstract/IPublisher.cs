using Restaurant.Models;
using Restaurant.Workers;

namespace Restaurant.Infrastructure.Abstract
{
    public interface IPublisher
    {
        void Publish(Topics topic, Order message);
    }
}