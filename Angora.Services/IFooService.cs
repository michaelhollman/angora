using System;
using System.ServiceModel;

namespace Angora.Services
{
    [ServiceContract]
    public interface IFooService
    {
        [OperationContract]
        string DoSomething(string value);
    }
}
