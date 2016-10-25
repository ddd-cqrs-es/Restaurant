using System;
using Restaurant.Infrastructure;
using Restaurant.Models;

namespace Restaurant.Events
{
    public class OrderPlaced : Event, ITTLMessage
    {
        public Order Order { get; set; }

        public DateTime ShoulBeProcessesdBefore
        {
            get
            {
                return ((ITTLMessage)Order).ShoulBeProcessesdBefore;
            }
        }

        public OrderPlaced(Order order)
        {
            Order = order;
        }
    }
}