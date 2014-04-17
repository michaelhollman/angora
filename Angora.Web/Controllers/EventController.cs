using System;
using System.Linq;
using System.Web.Mvc;
using Angora.Data.Models;
using Angora.Services;
using Angora.Web.Models;
using Microsoft.AspNet.Identity;
using Angora.Data;

namespace Angora.Web.Controllers
{
    public class EventController : Controller
    {
        private IEventService _eventService;
        private IUnitOfWork _unitOfWork;

        public EventController(IEventService eventService, IUnitOfWork unitOfWork)
        {
            _eventService = eventService;
            _unitOfWork = unitOfWork;
        }

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
            //Google reverseGeo(model.Location);
            //DateTime eventTime = DateTime.Parse(model.StartDateTime);

            //alot of these will probs have to change
            //I didn't use view models this summer so this is new stuff
            Event newEvent = new Event()
            {
                UserId = User.Identity.GetUserId(),
                Name = model.Name,
                Description = model.Description,
                // this will have to change when google stuff added
                Location = model.Location,
                StartDateTime = model.StartDateTime,
                EndDateTime = model.EndDateTime,
                Tags = model.Tags.Select(t => t.ToTag()).ToList(),
                CreationTime = DateTime.UtcNow
            };

            _eventService.Create(newEvent);
            _unitOfWork.SaveChanges();

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
                StartDateTime = theEvent.StartDateTime,
                Tags = theEvent.Tags.Select(t => t.ToString()).ToList()
            };
            return View(model);
        }

        [Authorize]
        public ActionResult EditEvent(EditEventViewModel model)
        {
            var e = _eventService.FindById(model.EventId);
            e.Name = model.Name;
            e.Description = model.Description;
            e.Location = model.Location;
            e.StartDateTime = model.StartDateTime;
            e.Tags = model.Tags.Select(t => t.ToTag()).ToList();
            _eventService.Edit(e);
            _unitOfWork.SaveChanges();

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