using System;
using Angora.Data;

namespace Angora.Services
{
    public class FooService : ServiceBase, IFooService
    {
        public string DoSomething(string value)
        {
            var foo = new Foo {SomeAttribute = value};

            var fooRepo = RepositoryFactory.NewRepository<Foo>();
            fooRepo.Insert(foo);
            fooRepo.SaveChanges();



            return string.Format("You did something to {0}", value.ToString());
        }
    }
}
