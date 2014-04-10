using System.Collections.Generic;
using Angora.Data.Models;

namespace Angora.Services
{
    public interface IEventService
    {
        long Create(Event newEvent);
        long Edit(Event oldEvent);
        void Delete(Event oldEvent);
        Event FindById(long id);
        IEnumerable<Event> FindEventsByUserId(string userId);
    }
}
