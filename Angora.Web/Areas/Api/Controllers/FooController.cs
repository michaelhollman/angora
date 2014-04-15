using System.Web.Http;
using Angora.Services;

namespace Angora.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/foo")]
    public class FooController : ApiController
    {

        private IEventService _eventService;

        public FooController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [Route("test")]
        public string Trolllolol(string q = "default")
        {
            if (_eventService != null)
            {
                return q;
            }
            return "EVENT SERVICE WAS NULL OH NOES!";
        }
    }
}