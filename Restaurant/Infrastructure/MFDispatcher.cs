using System.Collections.Generic;
using System.Threading;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class MFDispatcher : IOrderHandler
    {
        private readonly IEnumerable<QueuedHandler> _queuedHandlers;

        public MFDispatcher(IEnumerable<QueuedHandler> queuedHandlers)
        {
            _queuedHandlers = queuedHandlers;
        }

        public void HandleOrder(Order order)
        {
            while (true)
            {
                var managedToDispatch = false;

                foreach (var queuedHandler in _queuedHandlers)
                {
                    if (queuedHandler.QueueLength < 5)
                    {
                        managedToDispatch = true;
                        queuedHandler.HandleOrder(order);
                    }
                }

                if (!managedToDispatch)
                {
                    Thread.Sleep(200);
                    continue;
                }

                break;
            }
        }
    }
}