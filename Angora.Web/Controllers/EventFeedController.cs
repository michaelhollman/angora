using System.Web.Mvc;
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
        [Route("Events")]
        public ActionResult Index()
        {
            EventFeedViewModel model = new EventFeedViewModel();
            model.Events = _eventService.FindEventsByUserId(User.Identity.GetUserId());
            return View(model);
        }
    }
}