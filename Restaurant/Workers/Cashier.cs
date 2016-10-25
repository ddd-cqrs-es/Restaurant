using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Restaurant.Workers
{
    public class Cashier : IOrderHandler
    {
        private readonly Dictionary<string, Order> _outstandingOrders = new Dictionary<string, Order>();
        private readonly IOrderHandler _orderHandler;

        public Cashier(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }
        public void HandleOrder(Order order)
        {
            _outstandingOrders.Add(Guid.NewGuid().ToString(), order);
        }

        public void Pay(string orderId)
        {
            Thread.Sleep(100);

            var order = _outstandingOrders[orderId];
            order.Paid = true;

            _outstandingOrders.Remove(orderId);
            _orderHandler.HandleOrder(order);
        }

        public IEnumerable<string> GetOutstandingOrders()
        {
            return _outstandingOrders.Keys.Select(x => x).ToList();
        }
    }
}
