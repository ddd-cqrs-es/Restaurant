using Should;
using System.Linq;
using Xunit;
using Restaurant.Models;
using System.Collections.Generic;

namespace Restaurant.Tests
{
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
        public void ShouldSetTimeTable()
        {
            var order = new Order();

            order.TableNumber = 17;
            order.TableNumber.ShouldEqual(17);
        }

        [Fact]
        public void ShouldAddOrderItem()
        {
            var order = new Order();

            order.AddItem(new OrderItem()
            {
                Description = "desciption",
                Price = 11m,
                Quantity = 3
            });

            order.Items.Count.ShouldEqual(1);
        }

        [Fact]
        public void ShouldAddOrderItems()
        {
            var order = new Order();

            order.AddItems(new List<OrderItem>() { new OrderItem()
            {
                Description = "desciption",
                Price = 11m,
                Quantity = 3
            },
            new OrderItem()
            {
                Description = "desciption",
                Price = 11m,
                Quantity = 3
            }});

            order.Items.Count.ShouldEqual(2);
        }

        [Fact]
        public void ShouldAddIngredient()
        {
            var order = new Order();

            order.AddIngredient("Ingredient");

            order.Ingredients.Count.ShouldEqual(1);
        }

        [Fact]
        public void ShouldAddIngredients()
        {
            var order = new Order();

            order.AddIngredients(new List<string> { "Ingredient1", "Ingredient2" });

            order.Ingredients.Count.ShouldEqual(2);
        }

        [Fact]
        public void ShouldGetTimeToCookMs()
        {
            var order = new Order(json);

            order.TimeToCookMs.ShouldEqual(300);
        }

        [Fact]
        public void ShouldSetTimeToCookMs()
        {
            var order = new Order();

            order.TimeToCookMs = 300;

            order.TimeToCookMs.ShouldEqual(300);
        }

        [Fact]
        public void ShouldGetTax()
        {
            var order = new Order(json);

            order.Tax.ShouldEqual(2.00m);
        }

        [Fact]
        public void ShouldSetTax()
        {
            var order = new Order(json);

            order.Tax = 2.00m;

            order.Tax.ShouldEqual(2.00m);
        }

        [Fact]
        public void ShouldGetTotal()
        {
            var order = new Order(json);

            order.Total.ShouldEqual(11.00m);
        }

        [Fact]
        public void ShouldSetTotal()
        {
            var order = new Order(json);

            order.Total = 11.00m;

            order.Total.ShouldEqual(11.00m);
        }

        [Fact]
        public void ShouldGetPaid()
        {
            var order = new Order(json);

            order.Paid.ShouldEqual(true);
        }

        [Fact]
        public void ShouldSetPaid()
        {
            var order = new Order(json);

            order.Paid = true;

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
    }
}
