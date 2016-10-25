using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Restaurant.Helpers;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Infrastructure;

namespace Restaurant.Models
{
    public class Order: ITTLMessage
    {
        private readonly JsonSerializer _serializer = new JsonSerializer()
        {
            Formatting = Formatting.None,
            StringEscapeHandling = StringEscapeHandling.Default,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly dynamic _jsonOrder;

        public int TableNumber
        {
            get { return Getter.TryGet<int>(() => _jsonOrder.tableNumber); }
            set { _jsonOrder.tableNumber = value; }
        }

        public int TimeToCookMs
        {
            get { return Getter.TryGet<int>(() => _jsonOrder.timeToCookMs); }
            set { _jsonOrder.timeToCookMs = value; }
        }

        public decimal Tax
        {
            get { return Getter.TryGet<decimal>(() => _jsonOrder.tax); }
            set { _jsonOrder.tax = value; }
        }

        public decimal Total
        {
            get { return Getter.TryGet<decimal>(() => _jsonOrder.total); }
            set { _jsonOrder.total = value; }
        }

        public bool Paid
        {
            get { return Getter.TryGet<bool>(() => _jsonOrder.paid); }
            set { _jsonOrder.paid = value; }
        }
        
        public IReadOnlyList<OrderItem> Items
        {
            get { return ((JArray)(_jsonOrder.items ?? new JArray())).Select(item => new OrderItem(item.ToString())).ToList(); }
            set { _jsonOrder.items = JToken.FromObject(value, _serializer); }
        }

        public IReadOnlyList<string> Ingredients
        {
            get { return ((JArray)(_jsonOrder.ingredients ?? new JArray())).Select(x => (string)x).ToList(); }
            set { _jsonOrder.ingredients = JToken.FromObject(value, _serializer); }
        }
        
        public Order() :this("{}")
        {
        }

        public Order(string json)
        {
            _jsonOrder = JObject.Parse(json);

            ShoulBeProcessesdBefore = DateTime.Now;
        }

        public void AddItem(OrderItem orderItem)
        {
            var items = Items.ToList();
            items.Add(orderItem);
            Items = items;
        }

        public void AddItems(List<OrderItem> orderItems)
        {
            var items = Items.ToList();
            items.AddRange(orderItems);
            Items = items;
        }

        public void AddIngredient(string ingridient)
        {
            var ingridients = Ingredients.ToList();
            ingridients.Add(ingridient);
            Ingredients = ingridients;
        }

        public void AddIngredients(List<string> orderIngridients)
        {
            var ingridients = Ingredients.ToList();
            ingridients.AddRange(orderIngridients);
            Ingredients = ingridients;
        }

        public string ToJsonString()
        {
            return JObject.FromObject(this, _serializer).ToString();
        }

        public DateTime ShoulBeProcessesdBefore { get; set; }
    }
}
