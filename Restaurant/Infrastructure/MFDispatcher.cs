using System.Collections.Generic;
using System.Threading;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class MFDispatcher<T> : IHandler<T>
    {
        private readonly IEnumerable<QueuedHandler<T>> _queuedHandlers;

        public MFDispatcher(IEnumerable<QueuedHandler<T>> queuedHandlers)
        {
            _queuedHandlers = queuedHandlers;
        }

        public void Handle(T message)
        {
            while (true)
            {
                foreach (var queuedHandler in _queuedHandlers)
                {
                    if (queuedHandler.QueueLength < 5)
                    {
                        queuedHandler.Handle(message);
                        return;
                    }
                }

                Thread.Sleep(200);
            }
        }
    }
}