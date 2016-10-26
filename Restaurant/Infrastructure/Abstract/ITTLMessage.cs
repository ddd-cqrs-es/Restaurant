using System;

namespace Restaurant.Infrastructure.Abstract
{
    public interface ITTLMessage
    {
        DateTime ShoulBeProcessesdBefore { get; }

        string CorrelationId { get; }
    }
}