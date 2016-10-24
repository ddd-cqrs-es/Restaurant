using Newtonsoft.Json.Linq;
using Should;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Restaurant.Tests
{
    public class Getter
    {
        public static T TryGet<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (RuntimeBinderException)
            {
                return default(T);
            }
        }
    }
    public class Order
    {
        public class OrderItem
        {
            private dynamic _json;

            public OrderItem(string json)
            {
                _json = JObject.Parse(json);
            }

            [JsonProperty(PropertyName ="description")]
            public string Description {
                get { return Getter.TryGet<string>(() => { return _json.description; }); }
                set { _json.description = JToken.FromObject(value); }
            }

            public int Quantity {
                get { return Getter.TryGet<int>(() => { return _json.quantity; }); }
                set { _json.quantity = value; }
            }
            public decimal Price { get { return Getter.TryGet<decimal>(() => { return _json.price; }); } }
        }

        private dynamic _jsonOrder;

        public int TableNumber {
            get { return Getter.TryGet<int>(() => { return _jsonOrder.tableNumber; }); }
            set { _jsonOrder.tableNumber = value; }
        }
        public int TimeToCookMs {
            get { return Getter.TryGet<int>(() => { return _jsonOrder.timeToCookMs; }); }
            set { _jsonOrder.timeToCookMs = value; }
        }
        public decimal Tax {
            get { return Getter.TryGet<decimal>(() => { return _jsonOrder.tax; }); }
            set { _jsonOrder.tax = value; }
        }
        
        public decimal Total {
            get { return Getter.TryGet<decimal>(() => { return _jsonOrder.total; }); }
            set { _jsonOrder.total = value; }
        }
        
        public bool Paid
        {
            get { return Getter.TryGet<bool>(() => { return _jsonOrder.paid; }); }
            set { _jsonOrder.paid = value; }
        }
        
        public void ItemsAdd(OrderItem item)
        {
            _jsonOrder.items.Add(item);
        }

        public List<OrderItem> Items
        {
            get { return ((JArray)(_jsonOrder.items?? new JArray())).Select(item => new OrderItem(item.ToString())).ToList(); }
            set {
                if(_jsonOrder.items == null)
                {
                    _jsonOrder.items = new JArray();
                }
                
                _jsonOrder["items"] = JToken.FromObject(value);
            }
        }
        
        public List<string> Ingredients
        {
            get { return ((JArray)_jsonOrder.ingredients).Select(x => (string)x).ToList(); }
            set {
                if(_jsonOrder.ingredients == null)
                {
                    _jsonOrder.ingredients = new List<string>();
                }
                _jsonOrder["ingredients"] = JToken.FromObject(value);
                }
        }

        public Order(string json)
        {
            _jsonOrder = JObject.Parse(json);
        }

        public string ToJsonString()
        {
            var newJson = JObject.FromObject(this, new JsonSerializer()
            {
                Formatting = Formatting.None,
                StringEscapeHandling = StringEscapeHandling.Default,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return JObject.FromObject(this).ToString();
        }
    }

    

    public class OrderTests
    {
        private string json = @"{
            tableNumber: 17, 
            items: [{description:""razor balde apple"", quantity: 3, price: 9.00}], 
            ingredients: [""foo"", ""bar""],
            timeToCookMs: 300, 
            tax: 2.00, 
            total: 11.00, 
            paid: true}";

        [Fact]
        public void ShouldNotMapUknownProperties()
        {
            string json = @"{
            tableNumber: 17, 
            items: [{description:""razor balde apple"", quantity: 3, price: 9.00}], 
            ingredients: [""foo"", ""bar""],
            timeToCookMs: 300, 
            tax: 2.00, 
            total: 11.00}";

            var order = new Order(json);

            order.Paid.ShouldEqual(false);
            order.Paid = true;
            order.Paid.ShouldEqual(true);
        }

        [Fact]
        public void ShouldJsonWithoutItemsToEmptyList()
        {
            string json = @"{
            tableNumber: 17, 
            ingredients: [""foo"", ""bar""],
            timeToCookMs: 300, 
            tax: 2.00, 
            total: 11.00}";

            var order = new Order(json);

            order.Items.Count.ShouldEqual(0);
        }


        [Fact]
        public void ShouldGetTimeTable()
        {
            var order = new Order(json);

            order.TableNumber.ShouldEqual(17);
        }

        [Fact]
        public void ShouldGetTimeToCookMs()
        {
            var order = new Order(json);

            order.TimeToCookMs.ShouldEqual(300);
        }

        [Fact]
        public void ShouldGetTax()
        {
            var order = new Order(json);

            order.Tax.ShouldEqual(2.00m);
        }

        [Fact]
        public void ShouldGetTotal()
        {
            var order = new Order(json);

            order.Total.ShouldEqual(11.00m);
        }

        [Fact]
        public void ShouldGetPaid()
        {
            var order = new Order(json);

            order.Paid.ShouldEqual(true);
        }

        [Fact]
        public void ShouldGetOrderItems()
        {
            var order = new Order(json);

            order.Items.Count.ShouldEqual(1);
            order.Items[0].Description.ShouldEqual("razor balde apple");
            order.Items[0].Quantity.ShouldEqual(3);
            order.Items[0].Price.ShouldEqual(9.00m);
        }

        [Fact]
        public void ShouldGetIngredients()
        {
            var order = new Order(json);

            order.Ingredients.Count.ShouldEqual(2);
            order.Ingredients[0].ShouldEqual("foo");
            order.Ingredients[1].ShouldEqual("bar");
        }

        [Fact]
        public void ShouldChangeDescription()
        {
            var order = new Order(json);

            order.Items[0].Description = "new description";

            order.Items[0].Description.SequenceEqual("new description");
        }
        //[Fact]
        //public void ShouldSerializeDeserialize()
        //{
        //    var order = new Order(json);
        //    var newJson = JObject.FromObject(order, new JsonSerializer() {
        //        Formatting = Formatting.None, StringEscapeHandling = StringEscapeHandling.Default, ContractResolver = new CamelCasePropertyNamesContractResolver()
        //    });

        //    var newOrder = new Order(newJson.ToString());

        //    newJson.ToString().ShouldEqual<string>(json);
        //}
    }
}
