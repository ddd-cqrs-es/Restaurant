using System.Collections.Generic;

namespace Restaurant.Models
{
    public class Recipe
    {
        public string DishName { get; set; }
        public List<string> Ingredients { get; set; }
        public int TimeToPrepare { get; set; }

        public Recipe()
        {
            Ingredients = new List<string>();
        }
    }
}
