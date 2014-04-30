using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data.Models;

namespace Angora.Services
{
    public interface IPostService
    {
        Post Create(Post post);
        Post FindById(long id);
        IEnumerable<Post> FindPostsForEvent(long eventId);
        void AddOrUpdatePostToEvent(long eventId, Post post);
    }
}
