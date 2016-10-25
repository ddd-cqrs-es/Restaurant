using Restaurant.Models;

namespace Restaurant.Workers.Abstract
{
    public interface IOrderHandler
    {
        void HandleOrder(Order order);
    }
}
