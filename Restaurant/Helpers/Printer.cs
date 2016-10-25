using Restaurant.Models;
using Restaurant.Workers.Abstract;
using System;

namespace Restaurant.Helpers
{
    public class Printer<T> : IHandler<T> where T : Order
    {
        public void Handle(T orderCooked)
        {
            Console.WriteLine(orderCooked.ToJsonString());
        }
    }
}
