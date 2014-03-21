using Angora.Data.Models;
using Angora.Services;
using Angora.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;

namespace Angora.Web.Controllers
{
    public class EventController : Controller
    {
        private IEventService _eventService;

        public EventController (IEventService eventService){
            _eventService = eventService;
        }

        public EventController() : this(ServiceManager.GetService<IEventService>()) {}
        //
        // GET: /Event/
        public ActionResult Index()
        {
            return RedirectToAction("Index", "EventFeed");
        }

        [Authorize]
        public ActionResult Create()
        {
                NewEventViewModel newModel = new NewEventViewModel();
                return View(newModel);
        }

        [Authorize]
        public ActionResult CreateEvent(NewEventViewModel model)
        {
            //long userId = (long)Membership.GetUser().ProviderUserKey;
            //Google reverseGeo(model.Location);
            //DateTime eventTime = DateTime.Parse(model.Time);

            //alot of these will probs have to change
            //I didn't use view models this summer so this is new stuff
            Event newEvent = new Event()
            {
                UserId = User.Identity.GetUserId(),
                Name = model.Name,
                Description = model.Description,
                // this will have to change when google stuff added
                Location = model.Location,
                Time = model.Time,
                Tags = model.Tags,
                CreationTime = DateTime.UtcNow
            };

            _eventService.Create(newEvent);

            return RedirectToAction("Index", "EventFeed");
        }

        [Authorize]
        public ActionResult Edit(long id)
        {
            var theEvent = _eventService.FindById(id);
            EditEventViewModel model = new EditEventViewModel()
            {
                EventId = theEvent.Id,
                Name = theEvent.Name,
                Description = theEvent.Description,
                Location = theEvent.Location,
                Time = theEvent.Time,
                Tags = theEvent.Tags
            };
            return View(model);
        }

        //Doesn't work yet. Waiting on a method it uses to be refactored
        [Authorize]
        public ActionResult EditEvent(EditEventViewModel model)
        {
            var e = _eventService.FindById(model.EventId);
            e.Name = model.Name;
            e.Description = model.Description;
            e.Location = model.Location;
            e.Time = model.Time;
            e.Tags = model.Tags;
            _eventService.Edit(e);

            return RedirectToAction("Index", "EventFeed");
        }

        [Authorize]
        public ActionResult ViewEvent(long id)
        {
            Event eventView = _eventService.FindById(id);

            return View();
        }

	}
}