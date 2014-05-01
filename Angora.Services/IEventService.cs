using System.Collections.Generic;
using Angora.Data.Models;

namespace Angora.Services
{
    public interface IEventService
    {
        Event Create(Event newEvent);
        void Update(Event oldEvent);
        void Delete(Event oldEvent);
        void Delete(long id);

        Event FindById(long id);
        IEnumerable<Event> FindEventsCreatedByUser(string userId);
        IEnumerable<Event> FindEventsWithBlobs();
    }
}
