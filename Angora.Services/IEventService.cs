using System.Collections.Generic;
using System.ServiceModel;
using Angora.Data.Models;


namespace Angora.Services
{
    [ServiceContract]
    public interface IEventService
    {
        [OperationContract]
        long Create(Event newEvent);

        [OperationContract]
        long Edit(Event oldEvent);

        //not sure if we should be returning anythang
        [OperationContract]
        void Delete(Event oldEvent);

        [OperationContract]
        Event FindById(long id);

        [OperationContract]
        IEnumerable<Event> FindEventsByUserId(string userId);
    }
}
