using Microsoft.CSharp.RuntimeBinder;
using System;

namespace Restaurant.Helpers
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
}
