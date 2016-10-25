using Restaurant.Models;

namespace Restaurant.Workers.Abstract
{
    public interface IHandle<T>
    {
        void Handle(T message);
    }
}
