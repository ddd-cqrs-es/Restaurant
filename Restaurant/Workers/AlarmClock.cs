using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Workers.Abstract;

namespace Restaurant.Workers
{
    public class AlarmClock : IHandler<FutureMessage>, IStartable
    {
        private readonly IPublisher _publisher;
        private readonly List<FutureMessage> _messages = new List<FutureMessage>();
        private object _lock = new object();

        public AlarmClock(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(FutureMessage message)
        {
            lock (_lock)
            {
                _messages.Add(message);
            }
        }

        public void Start()
        {
            Task.Run(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));

                        lock (_lock)
                        {
                            var messages = new List<FutureMessage>();

                            foreach (var message in _messages.Where(message => message.TimeToBeDelivered >= DateTime.Now))
                            {
                                _publisher.Publish(message.MessageToDeliver);
                                messages.Add(message);
                            }

                            foreach (var message in messages)
                            {
                                _messages.Remove(message);
                            }
                        }
                    }
                });
        }
    }
}