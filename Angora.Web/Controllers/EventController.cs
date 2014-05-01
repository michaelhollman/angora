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
using Facebook;

namespace Angora.Web.Controllers
{
    [Authorize]
    [RoutePrefix("event")]
    public class EventController : Controller
    {
        private IEventService _eventService;
        private IAngoraUserService _userService;
        private IFooCDNService _fooCDNService;
        private IPostService _postService;
        private IRSVPService _rsvpService;
        private IUnitOfWork _unitOfWork;

        public EventController(IEventService eventService, IAngoraUserService userService, IFooCDNService fooCDNService, IPostService postService, IRSVPService rsvpService, IUnitOfWork unitOfWork)
        {
            _eventService = eventService;
            _userService = userService;
            _fooCDNService = fooCDNService;
            _postService = postService;
            _rsvpService = rsvpService;
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
            var vent = _eventService.FindById(id);
            vent.Posts = vent.Posts != null ? vent.Posts.OrderByDescending(p => p.PostTime).ToList() : new List<Post>();
            vent.RSVPs = vent.RSVPs ?? new List<RSVP>();
            var viewer = await _userService.FindUserById(User.Identity.GetUserId());

            var viewerRSVP = vent.RSVPs.SingleOrDefault(r => r.User.Id.Equals(viewer.Id));
            var viewerRSVPResponse = viewerRSVP != null ? viewerRSVP.Response : RSVPStatus.NoResponse;

            var counts = new Dictionary<RSVPStatus, int>();
            counts.Add(RSVPStatus.Yes, vent.RSVPs.Count(r => r.Response == RSVPStatus.Yes));
            counts.Add(RSVPStatus.No, vent.RSVPs.Count(r => r.Response == RSVPStatus.No));
            counts.Add(RSVPStatus.Maybe, vent.RSVPs.Count(r => r.Response == RSVPStatus.Maybe));

            var model = new EventViewModel
            {
                Event = vent,
                ViewerIsCreator = User.Identity.GetUserId().Equals(vent.Creator.Id),
                Viewer = viewer,
                ViewerRSVP = viewerRSVPResponse,
                RSVPCounts = counts
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
                await _fooCDNService.PostToBlob(blob, pictureData, picture.FileName);

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
                PostTime = DateTime.Now,
            };
            if (shareOnFacebook) {
                AngoraUser user = await _userService.FindUserById(User.Identity.GetUserId());
                string e = _eventService.FindById(id).Name;
                var fbUser = new FacebookClient(user.FacebookAccessToken);
                var parameters = new Dictionary<string, object>{
                        {"message", "I posted this to the event '" + e + "' on Auderus: \n" + text}};

                if (picture != null){
                     parameters.Add("picture", post.MediaItem.GetUrl());
                }

                fbUser.Post("me/feed", parameters);
            }
            _postService.AddOrUpdatePostToEvent(id, post);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [Route("{id}/rsvp")]
        public async Task<ActionResult> RSVP(long id, RSVPStatus response)
        {
            var vent = _eventService.FindById(id);

            var rsvp = vent.RSVPs != null ? vent.RSVPs.SingleOrDefault(r => r.User.Id.Equals(User.Identity.GetUserId())) : null;
            rsvp = rsvp ?? new RSVP
            {
                User = await _userService.FindUserById(User.Identity.GetUserId()),
            };
            rsvp.Response = response;

            _rsvpService.AddOrUpdateRSVPToEvent(id, rsvp);
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

            return RedirectToAction("Details", "Event", new { id = model.Event.Id });
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