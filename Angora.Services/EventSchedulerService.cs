using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data;
using Angora.Data.Models;

namespace Angora.Services
{
    public class EventSchedulerService : ServiceBase, IEventSchedulerService
    {
        private IEventService _eventService { get; set; }
        private IAngoraUserService _userService { get; set; }

        public EventSchedulerService(IEventService eventService, IAngoraUserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }



        public void SetResponse(long eventId, AngoraUser user, SchedulerResponse response, DateTime time)
        {
            var vent = _eventService.FindById(eventId);
            vent.Scheduler = vent.Scheduler ?? new EventScheduler();

            var resp = vent.Scheduler.Responses.FirstOrDefault(r => r.User.Id.Equals(user.Id) && r.Time.CompareTo(time) == 0);

            if (resp == null)
            {
                vent.Scheduler.Responses.Add(new EventSchedulerResponse
                {
                    User = user,
                    Response = response,
                    Time = time
                });
            }
            else
            {
                resp.Response = response;
            }

            _eventService.Update(vent);
        }


        public void AddProposedTimeToEvent(long eventId, EventTime evTime)
        {
            var vent = _eventService.FindById(eventId);
            vent.Scheduler = vent.Scheduler ?? new EventScheduler();

            if (!vent.Scheduler.ProposedTimes.Any(t => t.StartTime.CompareTo(evTime.StartTime) == 0))
            {
                vent.Scheduler.ProposedTimes.Add(evTime);
            }

            _eventService.Update(vent);
        }

        public void FinalizeTime(long eventId, DateTime time)
        {
            AddProposedTimeToEvent(eventId, time.AsEventTime());
            var vent = _eventService.FindById(eventId);

            var evTime = vent.Scheduler.ProposedTimes.FirstOrDefault(t => t.StartTime.CompareTo(time) == 0);
            vent.Scheduler.IsTimeSet = true;
            vent.EventTime = new EventTime
            {
                StartTime = time,
                DurationInMinutes = evTime == null || evTime.DurationInMinutes == 0 ? 60 : evTime.DurationInMinutes
            };
            _eventService.Update(vent);
        }

    }
}
