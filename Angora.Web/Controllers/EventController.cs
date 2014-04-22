using System;
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

namespace Angora.Web.Controllers
{
    public class EventController : Controller
    {
        private IEventService _eventService;
        private IUnitOfWork _unitOfWork;

        public EventController (IEventService eventService, IUnitOfWork unitOfWork){
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
            NewEventViewModel newModel = new NewEventViewModel { Latitude = "0", Longitude = "0" };
                return View(newModel);
        }

        [Authorize]
        public ActionResult CreateEvent(NewEventViewModel model, string lat, string lng)
        {
            //Google reverseGeo(model.Location);
            //DateTime eventTime = DateTime.Parse(model.StartDateTime);
            string location = Locate(model.Location);
            Event newEvent = new Event()
            {
                UserId = User.Identity.GetUserId(),
                Name = model.Name,
                Description = model.Description,
                // this will have to change when google stuff added
                Location = location,
                StartDateTime = model.StartDateTime,
                EndDateTime = model.EndDateTime,
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
            EditEventViewModel model = new EditEventViewModel()
            {
                EventId = theEvent.Id,
                Name = theEvent.Name,
                Description = theEvent.Description,
                Location = ReverseGeocode(theEvent.Location),
                StartDateTime = theEvent.StartDateTime,
                EndDateTime = theEvent.EndDateTime,
                Tags = theEvent.Tags
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
            e.Tags = model.Tags;
            _eventService.Edit(e);
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

        private static string Locate(string address)
        {
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            string location;

            if (xdoc.Element("GeocodeResponse").Element("status").Value.Equals("OK")) { 
                var locationElement = result.Element("geometry").Element("location");
                var lat = locationElement.Element("lat");
                var lng = locationElement.Element("lng");
            
                location = lat.Value + "," + lng.Value;
            }
            else
            {
                location = "Not valid location";
            }

            return location;
        }

        private static string ReverseGeocode(string latlng)
        {
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?latlng={0}&sensor=false", latlng);

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            string location;

            var addressElement = result.Element("formatted_address");

            location = addressElement.Value;

            return location;
        }

	}
}