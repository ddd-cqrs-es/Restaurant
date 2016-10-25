using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Restaurant.Workers
{
    public class Cook : IOrderHandler
    {
        private readonly IOrderHandler _orderHandler;
        private readonly Recipe[] _cookBook = {
            new Recipe
            {
                TimeToPrepare = 50,
                DishName = "pizza",
                Ingredients = new List<string> { "cheese", "tomato", "mushrooms" }
            },
            new Recipe
            {
                TimeToPrepare = 100,
                DishName = "pasta",
                Ingredients = new List<string> { "pasta", "tomato", "meet" }
            },
            new Recipe
            {
                TimeToPrepare = 10,
                DishName = "beer"
            },
            new Recipe
            {
                TimeToPrepare = 20,
                DishName = "wine"
            }
        };

        public Cook(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }

        public void HandleOrder(Order order)
        {
            foreach (var item in order.Items)
            {
                var recipe = _cookBook.Single(c => c.DishName == item.Description);

                Thread.Sleep(recipe.TimeToPrepare);

                order.AddIngredients(recipe.Ingredients);
                order.TimeToCookMs += recipe.TimeToPrepare;
            }

            _orderHandler.HandleOrder(order);
        }
    }
}
