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
using System.Web;
using System.IO;
using Angora.Data;

namespace Angora.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private IAngoraUserService _userService;
        private IEventService _eventService;
        private IFooCDNService _fooCDNService;
        private IPostService _postService;
        private IUnitOfWork _unitOfWork;

        public UserController(IAngoraUserService userService, IEventService eventService, IFooCDNService fooCDNService, IPostService postService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _eventService = eventService;
            _fooCDNService = fooCDNService;
            _postService = postService;
            _unitOfWork = unitOfWork;
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
            return events.ToList();
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

        [HttpPost, Route("upload/{eventId}")]
        public async Task<IHttpActionResult> Upload(long eventId)
        {
            byte[] picture;
            var fileName = string.Empty;

            if (Request.Content.IsMimeMultipartContent())
            {
                try
                {
                    var content = (await Request.Content.ReadAsMultipartAsync()).Contents.First();
                    fileName = content.Headers.ContentDisposition.FileName.TrimEnd('"').TrimStart('"');
                    picture = await content.ReadAsByteArrayAsync();
                }
                catch (Exception e)
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, e));
                }
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
            }

            MediaItem mediaItem = null;
            if (picture.Length == 0)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The content was empty or unsuccessfully delivered"));
            }

            var extension = Path.GetExtension(fileName).TrimStart('.');
            string blob = _fooCDNService.CreateNewBlob(string.Format("image/{0}", extension));

            _fooCDNService.PostToBlob(blob, picture, fileName);

            mediaItem = new MediaItem
            {
                FooCDNBlob = blob,
                Size = (ulong)picture.LongLength,
                MediaType = MediaType.Photo
            };

            var post = new Post
            {
                User = await _userService.FindUserById(User.Identity.GetUserId()),
                MediaItem = mediaItem,
                PostText = string.Empty,
                PostTime = DateTime.UtcNow,
            };

            _postService.AddOrUpdatePostToEvent(eventId, post);
            _unitOfWork.SaveChanges();

            return Ok();
        }
    }
}
