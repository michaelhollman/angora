using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Angora.Services
{
    public class FooService : ServiceBase, IFooService
    {
        public string DoSomething(object value)
        {
            return string.Format("You did something to {0}", value.ToString());
        }
    }
}
