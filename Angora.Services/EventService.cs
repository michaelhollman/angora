using System.Collections.Generic;
using Angora.Data;
using Angora.Data.Models;

namespace Angora.Services
{
    public class EventService : ServiceBase, IEventService
    {
        private IRepository<Event> _eventRepo;
        private IRepository<EventTime> _eventTimeRepo;
        private IRepository<EventScheduler> _eventSchedulerRepo;
        private IRepository<Location> _locationRepo;

        public EventService(IRepository<Event> eventRepo, IRepository<EventTime> eventTimeRepo, IRepository<EventScheduler> eventSchedulerRepo, IRepository<Location> locationRepo)
        {
            _eventRepo = eventRepo;
            _eventTimeRepo = eventTimeRepo;
            _eventSchedulerRepo = eventSchedulerRepo;
            _locationRepo = locationRepo;
        }

        public Event Create(Event newEvent)
        {
            return _eventRepo.Insert(newEvent);
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
            return _eventRepo.GetById(id);
        }

        public IEnumerable<Event> FindEventsCreatedByUser(string userId)
        {
            IEnumerable<Event> events = _eventRepo.Find(e => e.Creator.Id.Equals(userId));
            return events;
        }
    }
}
