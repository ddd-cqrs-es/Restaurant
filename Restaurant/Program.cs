using Newtonsoft.Json.Linq;
using Restaurant.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restaurant
{
    public interface IHandleOrder
    {
        void HandleOrder(Order order);
    }


    public class Waiter
    {
        private IHandleOrder _orderHandler;
        public Waiter(IHandleOrder orderHandler)
        {
            _orderHandler = orderHandler;
        }

        public void PlaceOrder(int tableNumber, List<string> items)
        {
            var json = JObject.FromObject(new { TableNumber = tableNumber, Items = new List<object> { new { } } });

            var order = new Order(@"{}");
            order.TableNumber = tableNumber;
            order.Items = items.Select(x => new Order.OrderItem(@"{}")
            {
                Description = x
            }).ToList();

            _orderHandler.HandleOrder(order);
        }
    }

    public class Cook : IHandleOrder
    {
        private IHandleOrder _orderHandler;
        public Cook(IHandleOrder orderHandler)
        {
            _orderHandler = orderHandler;
        }
        public void HandleOrder(Order order)
        {
            Thread.Sleep(50);
            order.Ingredients = new List<string> { "cheese", "tomato", "mushrooms" };
            order.TimeToCookMs = 50;

            _orderHandler.HandleOrder(order);
        }
    }

    public class AssistantManager : IHandleOrder
    {
        private IHandleOrder _orderHandler;
        public AssistantManager(IHandleOrder orderHandler)
        {
            _orderHandler = orderHandler;
        }
        public void HandleOrder(Order order)
        {
            order.Tax = 2.00m;
            order.Total = 11.00m;
            _orderHandler.HandleOrder(order);
        }
    }

    public class Cashier : IHandleOrder
    {
        private Dictionary<string, Order> _outstandingOrders = new Dictionary<string, Order>();
        private IHandleOrder _orderHandler;
        public Cashier(IHandleOrder orderHandler)
        {
            _orderHandler = orderHandler;
        }
        public void HandleOrder(Order order)
        {
            _outstandingOrders.Add(Guid.NewGuid().ToString(), order);
        }

        public void Pay(string orderId)
        {
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

    public class PrintOrder : IHandleOrder
    {
        public void HandleOrder(Order order)
        {
            Console.Write(order.ToJsonString());
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var cashier = new Cashier(new PrintOrder());
            var waiter = new Waiter(new Cook(new AssistantManager(cashier)));
            waiter.PlaceOrder(1, new List<string> { "pizza" }); waiter.PlaceOrder(1, new List<string> { "pizza" }); waiter.PlaceOrder(1, new List<string> { "pizza" }); waiter.PlaceOrder(1, new List<string> { "pizza" }); waiter.PlaceOrder(1, new List<string> { "pizza" }); waiter.PlaceOrder(1, new List<string> { "pizza" }); waiter.PlaceOrder(1, new List<string> { "pizza" }); waiter.PlaceOrder(1, new List<string> { "pizza" });
            var unpaidOrders = cashier.GetOutstandingOrders();
            foreach (var orderId in unpaidOrders)
            {
                cashier.Pay(orderId);
            }
            
            Console.ReadKey();
        }
    }
}
