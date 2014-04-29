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
using System.Web;

namespace Angora.Web.Controllers
{
    [Authorize]
    [RoutePrefix("event")]
    public class EventController : Controller
    {
        private IEventService _eventService;
        private IAngoraUserService _userService;
        private IFooCDNService _fooCDNService;
        private IUnitOfWork _unitOfWork;

        public EventController(IEventService eventService, IAngoraUserService userService, IFooCDNService fooCDNService, IUnitOfWork unitOfWork)
        {
            _eventService = eventService;
            _userService = userService;
            _fooCDNService = fooCDNService;
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "EventFeed");
        }

        [Route("{id}")]
        public async Task<ActionResult> Details(long id)
        {
            var theEvent = _eventService.FindById(id);

            var model = new EventViewModel
            {
                Event = theEvent,
                ViewerIsCreator = User.Identity.GetUserId().Equals(theEvent.Creator.Id),
                Viewer = await _userService.FindUserById(User.Identity.GetUserId()),
            };

            return View("Details", model);
        }

        [HttpPost]
        [Route("{id}/post")]
        public async Task<ActionResult> Post(long id, string text, HttpPostedFileBase picture = null, bool shareOnFacebook = false, bool tweet = false)
        {
            MediaItem mediaItem = null;
            if (picture != null)
            {
                MemoryStream target = new MemoryStream();
                picture.InputStream.CopyTo(target);
                var pictureData = target.ToArray();

                var extension = Path.GetExtension(picture.FileName).TrimStart('.');
                string blob = _fooCDNService.CreateNewBlob(string.Format("image/{0}", extension));

                //async
                _fooCDNService.PostToBlob(blob, pictureData, picture.FileName);

                mediaItem = new MediaItem
                {
                    FooCDNBlob = blob,
                    Size = (ulong)pictureData.LongLength,
                    MediaType = MediaType.Photo
                };
            }

            var post = new Post
            {
                User = await _userService.FindUserById(User.Identity.GetUserId()),
                MediaItem = mediaItem,
                PostText = text,
                PostTime = DateTime.UtcNow,
            };

            var vent = _eventService.FindById(id);

            vent.Posts = vent.Posts ?? new List<Post>();
            vent.Posts.Add(post);

            _eventService.Update(vent);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Details", new { id = id });
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