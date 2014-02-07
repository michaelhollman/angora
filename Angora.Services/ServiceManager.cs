using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Services
{
    public static class ServiceManager
    {
        private delegate object Constructor();
        private static Dictionary<Type, object> Services;
        private static Dictionary<Type, Constructor> Constructors;

        static ServiceManager()
        {
            Services = new Dictionary<Type, object>();
            Constructors = new Dictionary<Type, Constructor>();

            Constructors.Add(typeof(IFooService), () => new FooService());
        }

        public static T GetService<T>()
        {
            InitService<T>();
            return (T)Services[typeof(T)];
        }

        private static void InitService<T>()
        {
            Type t = typeof(T);
            if (!Services.ContainsKey(t))
            {
                if (Constructors.ContainsKey(t))
                {
                    Services.Add(t, Constructors[t].DynamicInvoke());
                }
            }
        }

    }
}
