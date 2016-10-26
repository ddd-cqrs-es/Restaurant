namespace Restaurant.Workers.Abstract
{
    public interface IHandler<in T>
    {
        void Handle(T message);
    }
}
