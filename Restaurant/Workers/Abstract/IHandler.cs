using Restaurant.Models;

namespace Restaurant.Workers.Abstract
{
    public interface IHandler<T>
    {
        void Handle(T message);
    }
}
