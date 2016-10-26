using Restaurant.Messages;
using Restaurant.Workers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Workers
{
    public interface IMidget : IHandler<Message>
    {
        Action<string> CleanUp { get; set; }
    }
}
