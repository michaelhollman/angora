using System.Collections.Generic;
using Angora.Data;
using Angora.Data.Models;

namespace Angora.Services
{
    public class EventService : ServiceBase, IEventService
    {
        private GenericRepository<Event> _eventRepo;
        private GenericRepository<EventTime> _eventTimeRepo;
        private GenericRepository<EventScheduler> _eventSchedulerRepo;
        private GenericRepository<Location> _locationRepo;

        public EventService(GenericRepository<Event> eventRepo, GenericRepository<EventTime> eventTimeRepo, GenericRepository<EventScheduler> eventSchedulerRepo, GenericRepository<Location> locationRepo)
        {
            _eventRepo = eventRepo;
            _eventTimeRepo = eventTimeRepo;
            _eventSchedulerRepo = eventSchedulerRepo;
            _locationRepo = locationRepo;
        }

        public long Create(Event newEvent)
        {
            _eventRepo.Insert(newEvent);

            return newEvent.Id;
        }

        public void Update(Event e)
        {
            _eventRepo.Update(e);
            if (e.EventTime != null && e.EventTime.Id != 0)
                _eventTimeRepo.Update(e.EventTime);
            if (e.Scheduler != null && e.Scheduler.Id != 0)
                _eventSchedulerRepo.Update(e.Scheduler);
            if (e.Location != null && e.Location.Id != 0)
                _locationRepo.Update(e.Location);
        }

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

        public IEnumerable<Event> FindEventsCreatedByUser(string userId)
        {
            IEnumerable<Event> events = _eventRepo.Find(e => e.Creator.Id.Equals(userId));
            return events;
        }
    }
}
