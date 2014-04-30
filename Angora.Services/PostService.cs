using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data;
using Angora.Data.Models;

namespace Angora.Services
{
    public class PostService : IPostService
    {
        private IRepository<Post> _postRepo;
        private IEventService _eventService;

        public PostService(IRepository<Post> postRepo, IEventService eventService)
        {
            _postRepo = postRepo;
            _eventService = eventService;
        }

        public Post Create(Post post)
        {
            return _postRepo.Insert(post);
        }
        public Post FindById(long id)
        {
            return _postRepo.GetById(id);
        }
        public IEnumerable<Post> FindPostsForEvent(long eventId)
        {
            return _postRepo.Find(p => p.EventId == eventId);
        }

        public void AddOrUpdatePostToEvent(long eventId, Post post)
        {
            var vent = _eventService.FindById(eventId);
            vent.Posts = vent.Posts ?? new List<Post>();

            if (post.Id == 0)
            {
                post = Create(post);
                vent.Posts.Add(post);
            }
            else
            {
                _postRepo.Update(post);
            }

            _eventService.Update(vent);
        }
    }
}
