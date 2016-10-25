﻿using Restaurant.Models;

namespace Restaurant.Events
{
    public class OrderPriced
    {
        public Order Order { get; set; }

        public OrderPriced(Order order)
        {
            Order = order;
        }
    }
}