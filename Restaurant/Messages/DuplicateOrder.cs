using System;
using Restaurant.Models;

namespace Restaurant.Messages
{
    public class DuplicateOrder : Message
    {
        public DuplicateOrder(DateTime shoulBeProcessesdBefore, string correlationId, string causationId) : base(shoulBeProcessesdBefore, correlationId, causationId)
        {
           
        }
    }
}