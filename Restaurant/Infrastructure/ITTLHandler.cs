using System;

namespace Restaurant.Infrastructure
{
    public interface ITTLMessage
    {
        DateTime ShoulBeProcessesdBefore { get; }
    }
}