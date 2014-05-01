using System;
using System.Linq;
using System.Web.Mvc;
using Angora.Data.Models;
using Angora.Services;
using Angora.Web.Models;
using Microsoft.AspNet.Identity;

namespace Angora.Web.Controllers
{
    public class EventFeedController : Controller
    {

        private IEventService _eventService;

        public EventFeedController(IEventService eventService)
        {
            _eventService = eventService;
        }

        //
        // GET: /EventFeed/
        [Route("events")]
        public ActionResult Index()
        {
            var model = new EventFeedViewModel();
            //var events = _eventService.FindEventsCreatedByUser(User.Identity.GetUserId());
            var allEvents = _eventService.GetAllEvents();

            var viewModels = allEvents.Select(e => new EventViewModel()
            {
                Event = e,
                ViewerIsCreator = User.Identity.GetUserId().Equals(e.Creator.Id),
            }).OrderBy(m => m.Event.EventTime != null ? m.Event.EventTime.StartTime : DateTime.MinValue).ThenBy(m => m.Event.CreationTime);

            model.ViewersEvents = viewModels.Where(m => m.ViewerIsCreator || (m.Event.RSVPs != null && m.Event.RSVPs.Any(r => r.User.Id.Equals(User.Identity.GetUserId()) && (r.Response == RSVPStatus.Yes || r.Response == RSVPStatus.Maybe))));
            model.OtherEvents = viewModels.Where(m => !model.ViewersEvents.Any(mm => mm.Event.Id == m.Event.Id));

            return View(model);
        }
    }
}