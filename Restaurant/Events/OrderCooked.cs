using System;
using Restaurant.Infrastructure;
using Restaurant.Models;

namespace Restaurant.Events
{
    public class OrderCooked : Event, ITTLMessage
    {
        public Order Order { get; set; }

        public DateTime ShoulBeProcessesdBefore
        {
            get
            {
                return ((ITTLMessage)Order).ShoulBeProcessesdBefore;
            }
        }

        public OrderCooked(Order order)
        {
            Order = order;
        }
    }
}