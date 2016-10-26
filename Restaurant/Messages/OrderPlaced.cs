using System;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Models;

namespace Restaurant.Messages
{
    public class OrderPlaced : Message
    {
        public Order Order { get; }
        
        public OrderPlaced(Order order, string causationId) :
            base(order.ShoulBeProcessesdBefore, order.Id, causationId)
        {
            Order = order;
        }
    }
}