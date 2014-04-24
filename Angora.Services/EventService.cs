using System.Collections.Generic;
using Angora.Data;
using Angora.Data.Models;

namespace Angora.Services
{
    public class EventService : ServiceBase, IEventService
    {
        private GenericRepository<Event> _eventRepo;

        public EventService(GenericRepository<Event> eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public long Create(Event newEvent)
        {
            _eventRepo.Insert(newEvent);

            return newEvent.Id;
        }

        public long Edit(Event oldEvent)
        {
            _eventRepo.Update(oldEvent);

            return oldEvent.Id;
        }

        //not sure if we should be returning anythang
        public void Delete(Event oldEvent)
        {
            _eventRepo.Delete(oldEvent);

        }

        public void Delete(long id)
        {
            Event e = FindById(id);

            _eventRepo.Delete(e);

        }

        public void DeleteById(long id)
        {
            _eventRepo.Delete(_eventRepo.GetById(id));
        }

        public Event FindById(long id)
        {
            Event thisEvent = _eventRepo.GetById(id);
            return thisEvent;
        }

        public IEnumerable<Event> FindEventsByUserId(string userId)
        {
            IEnumerable<Event> events = _eventRepo.Find((Event e) => e.UserId.Equals(userId));
            return events;
        }
    }
}
