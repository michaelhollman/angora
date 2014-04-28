using System;
using System.Linq;
using System.Web.Mvc;
using Angora.Data.Models;
using Angora.Services;
using Angora.Web.Models;
using Microsoft.AspNet.Identity;
using Angora.Data;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Globalization;
using System.Threading.Tasks;

namespace Angora.Web.Controllers
{
    [Authorize]
    [RoutePrefix("event")]
    public class EventController : Controller
    {
        private IEventService _eventService;
        private IAngoraUserService _userService;
        private IUnitOfWork _unitOfWork;

        public EventController(IEventService eventService, IAngoraUserService userService, IUnitOfWork unitOfWork)
        {
            _eventService = eventService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "EventFeed");
        }

        [Route("{id}")]
        public ActionResult Details(long id)
        {
            var theEvent = _eventService.FindById(id);

            var model = new EventViewModel
            {
                Event = theEvent,
                ViewerIsCreator = User.Identity.GetUserId().Equals(theEvent.Creator.Id)
            };

            return View("Details", model);
        }

        [HttpGet]
        [Route("new")]
        public ActionResult Create()
        {
            NewEventViewModel model = new NewEventViewModel();
            return View(model);
        }

        [HttpPost]
        [Route("new")]
        public async Task<ActionResult> CreateEvent(NewEventViewModel model)
        {
            var eventTime = new EventTime
            {
                StartTime = model.StartDateTime,
                DurationInMinutes = model.DurationHours * 60 + model.DurationMinutes
            };

            var scheduler = new EventScheduler
            {
                IsTimeSet = model.ScheduleNow,
                ProposedTimes = new List<EventTime>(),
                Responses = new List<EventSchedulerResponse>(),
            };

            var location = new Location
            {
                NameOrAddress = model.Location,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };

            var newEvent = new Event
            {
                Creator = await _userService.FindUserById(User.Identity.GetUserId()),
                Name = model.Name,
                Description = model.Description,
                Location = location,
                EventTime = model.ScheduleNow ? eventTime : null,
                Scheduler = scheduler,
                CreationTime = DateTime.UtcNow
            };

            _eventService.Create(newEvent);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Index", "EventFeed");
        }

        [HttpGet]
        [Route("{id}/edit")]
        public ActionResult Edit(long id)
        {
            var theEvent = _eventService.FindById(id);

            if (theEvent == null || !theEvent.Creator.Id.Equals(User.Identity.GetUserId()))
            {
                return RedirectToAction("Index", "EventFeed");
            }

            var model = new EventEditViewModel
            {
                Event = theEvent,
                DurationHours = theEvent.EventTime == null ? 0 : theEvent.EventTime.DurationInMinutes / 60,
                DurationMinutes = theEvent.EventTime == null ? 0 : theEvent.EventTime.DurationInMinutes % 60
            };

            return View(model);
        }

        [HttpPost]
        [Route("{id}/edit")]
        public ActionResult EditEvent(EventEditViewModel model)
        {
            model.Event.EventTime.DurationInMinutes = model.DurationHours * 60 + model.DurationMinutes;

            _eventService.Update(model.Event);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Index", "EventFeed");
        }

        [Route("{id}/delete")]
        public ActionResult DeleteEvent(long id)
        {
            _eventService.Delete(id);

            _unitOfWork.SaveChanges();

            return RedirectToAction("Index", "EventFeed");
        }
    }
}