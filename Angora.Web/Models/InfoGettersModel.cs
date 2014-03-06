using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Angora.Web.Models
{
    public class InfoGetterModel
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set;}
        public string Gender { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public string Link { get; set; }
        public List<Friend> FriendsList { get; set; }
        public string PictureUrl { get; set; }

    }

    public struct Friend
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string PictureUrl { get; set; }
    }
}