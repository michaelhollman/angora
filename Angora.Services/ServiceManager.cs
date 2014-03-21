using System;
using System.Collections.Generic;

namespace Angora.Services
{
    // TODO replace all of this with UNITY

    public static class ServiceManager
    {
        private delegate object Constructor();
        private static Dictionary<Type, object> Services;
        private static Dictionary<Type, Constructor> Constructors;

        static ServiceManager()
        {
            Services = new Dictionary<Type, object>();
            Constructors = new Dictionary<Type, Constructor>
            {
                {typeof (IFooService), () => new FooService()},
                {typeof (IAngoraUserService), () => new AngoraUserService()},
                {typeof (IFooCDNService), () => new FooCDNService()}
            };
        }

        public static T GetService<T>()
        {
            InitService<T>();
            return (T)Services[typeof(T)];
        }

        private static void InitService<T>()
        {
            var t = typeof(T);
            if (Services.ContainsKey(t)) return;
            if (Constructors.ContainsKey(t))
            {
                Services.Add(t, Constructors[t].DynamicInvoke());
            }
        }

    }
}
