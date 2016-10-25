using System;
using Restaurant.Models;
using Restaurant.Workers.Abstract;

namespace Restaurant.Infrastructure
{
    public class TTLHandler<T> : IHandle<T> where T: ITTLHandler
    {
        private readonly IHandle<T> _handler;

        public TTLHandler(IHandle<T> handler)
        {
            _handler = handler;
        }

        public void Handle(T message)
        {
            if (message.ShoulBeProcessesdBefore > DateTime.Now.Add(-TimeSpan.FromMinutes(0.5)))
            {
                _handler.Handle(message);
            }
            else
            {
                //todo
             //   Console.WriteLine($"skipping {message.Id}");
            }
        }
    }
}