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

        public PostService(IRepository<Post> postRepo)
        {
            _postRepo = postRepo;
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
    }
}
