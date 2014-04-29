using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Angora.Services;
using Microsoft.AspNet.Identity;
using Angora.Data.Models;
using Facebook;
using TweetSharp;

namespace Angora.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private IAngoraUserService _userService;
        private IEventService _eventService;

        public UserController(IAngoraUserService userService, IEventService eventService)
        {
            _userService = userService;
            _eventService = eventService;
        }

        // GET api/<controller>/login
        [HttpGet]
        [Route("login")]
        //TODO this is wide open to anyone, obviously.
        public AngoraUser Login(string provider, string providerKey)
        {
            var user = _userService.FindUser(new UserLoginInfo(provider, providerKey));
            
            //TODO if the user is not found, will return null
            return user;
        }

        [HttpGet]
        [Route("getEvents")]
        public List<Event> GetEvents(string userId)
        {
            var events = _eventService.FindEventsCreatedByUser(userId);
            return events.ToList<Event>();
        }

        [HttpPost]
        [Route("createEvent")]
        public void CreateEvent(Event newEvent)
        {
            _eventService.Create(newEvent);
        }

        [HttpPost]
        [Route("deleteEvent")]
        public void DeleteEvent(long eventId)
        {
            _eventService.Delete(_eventService.FindById(eventId));
        }

        [HttpPost, Route("upload")]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent()) { 
                throw new Exception();  //meow?
            }
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();
                //todo something with these things 

            }

            return Ok();
        }
        
        [HttpPost, Route("upload")]
        public async Task<IHttpActionResult> Upload(string lat, string lon)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new Exception();  //meow?
            }
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();
                //todo something with these things 

            }

            return Ok();
        }
    }
}
