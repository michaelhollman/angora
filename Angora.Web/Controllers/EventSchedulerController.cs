using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Angora.Data;
using Angora.Data.Models;
using Angora.Services;
using Angora.Web.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;


namespace Angora.Web.Controllers
{
    [Authorize]
    [RoutePrefix("event/{id}/findtime")]
    public class EventSchedulerController : Controller
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private IEventService _eventService { get; set; }
        private IEventSchedulerService _eventShedulerService { get; set; }
        private IAngoraUserService _userService { get; set; }


        public EventSchedulerController(IUnitOfWork unitOfWork, IEventService eventService, IEventSchedulerService eventSchedulerService, IAngoraUserService userService)
        {
            _unitOfWork = unitOfWork;
            _eventService = eventService;
            _eventShedulerService = eventSchedulerService;
            _userService = userService;
        }

        [Route("")]
        [HttpGet]
        public ActionResult Index(long id)
        {
            var theEvent = _eventService.FindById(id);
            if (theEvent == null)
            {
                return RedirectToAction("Index", "EventFeed");
            }

            theEvent.Scheduler = theEvent.Scheduler ?? new EventScheduler
            {
                IsTimeSet = theEvent.EventTime != null && theEvent.EventTime.StartTime != null
            };

            var model = new EventSchedulerViewModel();
            model.Event = theEvent;
            model.ViewerIsCreator = theEvent.Creator.Id.Equals(User.Identity.GetUserId());

            if (theEvent.Scheduler.IsTimeSet)
            {
                return RedirectToAction("Details", "Event", new { id = id });
            }

            model.Times = theEvent.Scheduler.ProposedTimes.Select(pt =>
            {
                var resp = theEvent.Scheduler.Responses.FirstOrDefault(r => r.Time.CompareTo(pt.StartTime) == 0 && r.User.Id.Equals(User.Identity.GetUserId()));

                return new EventSchedulerProposedTimeViewModel
                {
                    Time = pt.StartTime,
                    CurrentUserResponse = resp == null ? SchedulerResponse.NoResponse : resp.Response,
                    YesCount = theEvent.Scheduler.Responses.Count(r => r.Time.CompareTo(pt.StartTime) == 0 && r.Response == SchedulerResponse.Yes),
                    MaybeCount = theEvent.Scheduler.Responses.Count(r => r.Time.CompareTo(pt.StartTime) == 0 && r.Response == SchedulerResponse.Maybe),
                    NoCount = theEvent.Scheduler.Responses.Count(r => r.Time.CompareTo(pt.StartTime) == 0 && r.Response == SchedulerResponse.No)
                };
            }).OrderBy(t => t.Time).ToList();

            return View("FindTime", model);
        }

        [Route("")]
        [HttpPost]
        public ActionResult RSVP(long id, SchedulerResponse response)
        {

            // TODO -- do oodles of things here later, potentially with invites.

            return RedirectToAction("Index", "EventFeed");
        }

        [Route("find")]
        public async Task<ActionResult> FindTime(long id, DateTime dt, SchedulerResponse response)
        {

            var user = await _userService.FindUserById(User.Identity.GetUserId());

            _eventShedulerService.SetResponse(id, user, response, dt);

            _unitOfWork.SaveChanges();

            return RedirectToAction("Index");
        }

        [Route("add")]
        [HttpPost]
        public ActionResult AddTime(long id, DateTime startDateTime)
        {
            var newTime = new EventTime
            {
                StartTime = startDateTime,
                DurationInMinutes = 60
            };

            _eventShedulerService.AddProposedTimeToEvent(id, newTime);

            _unitOfWork.SaveChanges();

            return RedirectToAction("RSVP");
        }

        [Route("finalize")]
        public ActionResult FinalizeTime(long id, DateTime time)
        {
            _eventShedulerService.FinalizeTime(id, time);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}