using System.Web.Mvc;
using Angora.Services;
using Angora.Web.Models;
using Microsoft.AspNet.Identity;
using System.Linq;

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
            var events = _eventService.FindEventsCreatedByUser(User.Identity.GetUserId());
            model.Events = events.Select(e => new EventViewModel()
            {
                Event = e,
                ViewerIsCreator = User.Identity.GetUserId().Equals(e.Creator.Id),
            });
            return View(model);
        }
    }
}