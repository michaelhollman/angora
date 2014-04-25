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


namespace Angora.Web.Controllers
{
    [Authorize]
    [RoutePrefix("event/{id}/schedule")]
    public class EventSchedulerController : Controller
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private IEventService _eventService { get; set; }
        private IEventSchedulerService _eventShedulerService { get; set; }


        public EventSchedulerController(IEventService eventService, IEventSchedulerService eventSchedulerService)
        {
            _eventService = eventService;
            _eventShedulerService = eventSchedulerService;
        }

        [Route("")]
        public ActionResult Index(long id)
        {

            var theEvent = _eventService.FindById(id);
            if (theEvent == null)
            {
                return RedirectToAction("Index", "EventFeed");
            }

            var model = new EventSchedulerViewModel();

            model.Event = theEvent;
            model.ViewerOwnsEvent = theEvent.Creator.Id.Equals(User.Identity.GetUserId());

            return View(model);
        }
    }
}