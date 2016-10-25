using System;

namespace Restaurant.Infrastructure
{
    public interface ITTLHandler
    {
        DateTime ShoulBeProcessesdBefore { get; }
    }
}