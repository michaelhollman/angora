using System;
using Angora.Data;
using Angora.Data.Models;
using System.Collections.Generic;

namespace Angora.Services
{
    class EventService : ServiceBase, IEventService
    {
        long Create(Event newEvent)
        {
            var eventRepo = RepositoryFactory.NewRepository<Event>();
            eventRepo.Insert(newEvent);
            eventRepo.SaveChanges();

            return newEvent.Id;
        }

        long Edit(Event oldEvent)
        {
            var eventRepo = RepositoryFactory.NewRepository<Event>();
            eventRepo.Update(oldEvent);
            eventRepo.SaveChanges();

            return oldEvent.Id;
        }

        //not sure if we should be returning anythang
        void Delete(Event oldEvent)
        {
            var eventRepo = RepositoryFactory.NewRepository<Event>();
            eventRepo.Delete(oldEvent);
            eventRepo.SaveChanges();

        }

        Event FindById(long id)
        {
            var eventRepo = RepositoryFactory.NewRepository<Event>();
            Event thisEvent = eventRepo.GetById(id);

            return thisEvent;
        }

        List<Event> FindEventsByUserId(long userId)
        {
            var eventRepo = RepositoryFactory.NewRepository<Event>();
            List<Event> events = (List<Event>)eventRepo.Find((Event e) => e.Id == userId);
            return events;          
        }
    }
}
