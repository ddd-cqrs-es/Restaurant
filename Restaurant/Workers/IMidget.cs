using Restaurant.Messages;
using Restaurant.Workers.Abstract;
using System;

namespace Restaurant.Workers
{
    public interface IMidget : IHandler<Message>
    {
        Action<string> CleanUp { get; set; }
    }
}
