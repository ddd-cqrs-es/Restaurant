using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;

namespace Restaurant.Workers
{
    public class Cook : IHandler<CookFood>
    {
        private readonly IPublisher _publisher;
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

        private readonly int _time;

        public Cook(int time, IPublisher publisher)
        {
            _time = time;
            _publisher = publisher;
        }

        public void Handle(CookFood message)
        {
            foreach (var item in message.Order.Items)
            {
                var recipe = _cookBook.Single(c => c.DishName == item.Description);
                
                Thread.Sleep(_time);

                message.Order.AddIngredients(recipe.Ingredients);
                message.Order.TimeToCookMs += _time;
            }

            _publisher.Publish(new OrderCooked(message.Order, message.MessageId));
        }
    }
}
