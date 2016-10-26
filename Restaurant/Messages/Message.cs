using System;
using Restaurant.Infrastructure.Abstract;

namespace Restaurant.Messages
{
    public class Message : ITTLMessage
    {
        protected Message(DateTime shoulBeProcessesdBefore, string correlationId, string causationId)
        {
            ShoulBeProcessesdBefore = shoulBeProcessesdBefore;
            CorrelationId = correlationId;
            CausationId = causationId;
            MessageId = Guid.NewGuid().ToString();
        }

        public DateTime ShoulBeProcessesdBefore { get; }

        public string CorrelationId { get; }

        public string CausationId { get; set; }

        public string MessageId { get; private set; }
    }
}
