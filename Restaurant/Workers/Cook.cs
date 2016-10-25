using System;
using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Restaurant.Infrastructure.Abstract;

namespace Restaurant.Workers
{
    public class Cook<T> : IHandler<T> where T : Order
    {
        private static readonly Random Seed = new Random(DateTime.Now.Millisecond);
        private readonly string _name;
        private readonly IPublisher _orderPublisher;
        private readonly Recipe[] _cookBook = {
            new Recipe
            {
                TimeToPrepare = 200,
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

        private int _time;

        public Cook(int time, string name, IPublisher orderPublisher)
        {
            _time = time;
            _name = name;
            _orderPublisher = orderPublisher;
        }

        public void Handle(T order)
        {
            foreach (var item in order.Items)
            {
                var recipe = _cookBook.SingleOrDefault(c => c.DishName == item.Description);

                if (recipe == null)
                {
                    
                }
                
                Thread.Sleep(_time);

                order.AddIngredients(recipe.Ingredients);
                order.TimeToCookMs += _time;
            }

            _orderPublisher.Publish(Topics.FoodCooked, order);
        }
    }
}
