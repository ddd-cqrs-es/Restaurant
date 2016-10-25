using Newtonsoft.Json.Linq;
using Restaurant.Helpers;

namespace Restaurant.Models
{
    public class OrderItem
    {
        private dynamic _json;

        public OrderItem()
        {
            _json = JObject.Parse("{}");
        }

        public OrderItem(string json)
        {
            _json = JObject.Parse(json);
        }

        public string Description
        {
            get { return Getter.TryGet<string>(() => { return _json.description; }); }
            set { _json.description = JToken.FromObject(value); }
        }

        public int Quantity
        {
            get { return Getter.TryGet<int>(() => { return _json.quantity; }); }
            set { _json.quantity = value; }
        }

        public decimal Price
        {
            get { return Getter.TryGet<decimal>(() => { return _json.price; }); }
            set { _json.price = value; }
        }
    }
}
