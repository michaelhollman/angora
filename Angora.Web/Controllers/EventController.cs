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

        //
        // GET: /Event/
        public ActionResult Index()
        {
            return RedirectToAction("Index", "EventFeed");
        }

        [Authorize]
        public ActionResult Create()
        {
            NewEventViewModel newModel = new NewEventViewModel { Latitude = "0", Longitude = "0" };
            return View(newModel);
        }


        [Authorize]
        public async Task<ActionResult> CreateEvent(NewEventViewModel model, string lat, string lng)
        {
            EventTime eventTime = new EventTime
            {
                StartTime = model.StartDateTime,
                DurationInMinutes = model.DurationHours * 60 + model.DurationMinutes
            };

            Location location = new Location
            {
                NameOrAddress = model.Location,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };

            Event newEvent = new Event
            {
                Creator = await _userService.FindUserById(User.Identity.GetUserId()),
                Name = model.Name,
                Description = model.Description,
                Location = location,
                EventTime = eventTime,
                Tags = model.Tags,
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

            if (theEvent == null || !theEvent.Creator.Id.Equals(User.Identity.GetUserId()))
            {
                return RedirectToAction("Index", "EventFeed");
            }

            var model = new EventEditViewModel
            {
                Event = theEvent,
                DurationHours = theEvent.EventTime.DurationInMinutes / 60,
                DurationMinutes = theEvent.EventTime.DurationInMinutes % 60
            };

            return View(model);
        }

        [Authorize]
        public ActionResult EditEvent(EventEditViewModel model)
        {
            model.Event.EventTime.DurationInMinutes = model.DurationHours * 60 + model.DurationMinutes;

            _eventService.Update(model.Event);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Index", "EventFeed");
        }

        public ActionResult DeleteEvent(long id)
        {
            _eventService.Delete(id);

            _unitOfWork.SaveChanges();

            return RedirectToAction("Index", "EventFeed");
        }

        [Authorize]
        public ActionResult ViewEvent(long id)
        {
            Event eventView = _eventService.FindById(id);

            return View();
        }

        private static string GetCoordinates(string address)
        {
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            string location;

            if (xdoc.Element("GeocodeResponse").Element("status").Value.Equals("OK"))
            {
                var locationElement = result.Element("geometry").Element("location");
                var lat = locationElement.Element("lat");
                var lng = locationElement.Element("lng");

                location = lat.Value + "," + lng.Value;
            }
            else
            {
                location = "";
            }

            return location;
        }

    }
}