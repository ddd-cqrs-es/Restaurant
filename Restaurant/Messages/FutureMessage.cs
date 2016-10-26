using System;

namespace Restaurant.Messages
{
    public class FutureMessage : Message
    {
        public DateTime TimeToBeDelivered { get; set; }

        public FutureMessage(Message messageToDeliver, DateTime timeToBeDelivered) : base(DateTime.MaxValue, messageToDeliver.CorrelationId, messageToDeliver.MessageId)
        {
            MessageToDeliver = messageToDeliver;
            TimeToBeDelivered = timeToBeDelivered;
        }

        public Message MessageToDeliver { get; set; }
    }
}