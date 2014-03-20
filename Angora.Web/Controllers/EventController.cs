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
            return View();
        }

        public ActionResult Create()
        {

                NewEventViewModel newModel = new NewEventViewModel();
                return View(newModel);
        }

        public ActionResult CreateEvent(NewEventViewModel model)
        {
            //long userId = (long)Membership.GetUser().ProviderUserKey;
            //Google reverseGeo(model.Location);
            //DateTime eventTime = DateTime.Parse(model.Time);

            //alot of these will probs have to change
            //I didn't use view models this summer so this is new stuff
            Event newEvent = new Event()
            {
                UserId = 1,
                Name = model.Name,
                Description = model.Description,
                // this will have to change when google stuff added
                Location = model.Location,
                Time = model.Time,
                Tags = model.Tags,
                CreationTime = DateTime.UtcNow
            };

            _eventService.Create(newEvent);

            return View("Index");
        }

        public ActionResult EditEvent(EditEventViewModel model)
        {
            return View();
        }

        public ActionResult ViewEvent(long id)
        {
            EventViewModel eventView = new EventViewModel();

            return View(eventView);
        }

        private EventViewModel FindEventById(long id)
        {
           Event thisEvent = _eventService.FindById(id);

           EventViewModel eventView = new EventViewModel()
           {
               Event = thisEvent
           };

            return eventView;
        }
	}
}