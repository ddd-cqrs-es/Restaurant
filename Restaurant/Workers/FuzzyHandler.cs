using System;
using Restaurant.Workers.Abstract;

namespace Restaurant.Workers
{
    public class FuzzyHandler<T> : IHandler<T>
    {
        private static readonly Random Random = new Random();
        private readonly IHandler<T> _handler;
        private readonly int _dropMessagePct;
        private readonly int _duplicateMessagePct;
      
        public FuzzyHandler(IHandler<T> handler, int dropMessagePct, int duplicateMessagePct)
        {
            _handler = handler;
            _dropMessagePct = dropMessagePct;
            _duplicateMessagePct = duplicateMessagePct;
        }

        public void Handle(T message)
        {
            var rnd = Random.Next(100);
          
            if (rnd < _duplicateMessagePct)
            {
                _handler.Handle(message);
                _handler.Handle(message);
            }

            if (rnd < _dropMessagePct)
            {
                return;
            }

            _handler.Handle(message);
        }
    }
}