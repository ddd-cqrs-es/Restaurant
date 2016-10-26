using System;
using Restaurant.Messages;
using Restaurant.Workers.Abstract;

namespace Restaurant.Helpers
{
    public class Printer : IHandler<Message>
    {
        public void Handle(Message message)
        {
            Console.WriteLine($"MessageId: {message.MessageId} CorrelationId:{message.CorrelationId} CausationId:{message.CausationId} ");
        }
    }
}
