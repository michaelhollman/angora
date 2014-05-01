using System;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Angora.Data.Models
{
    public class AngoraUser : IdentityUser
    {
        public AngoraUser()
        {
            ProfilePictureUrl = "/Images/default-user-profile.png";
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Location { get; set; }
        public DateTime? Birthday { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string FacebookAccessToken { get; set; }
        public string TwitterAccessToken { get; set; }
        public string TwitterAccessSecret { get; set; }
    }

    public static class AngoraUserExtensions
    {
        public static bool IsLinkedWithFacebook(this AngoraUser user)
        {
            return user.Logins.Any(l => "Facebook".Equals(l.LoginProvider, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsLinkedWithTwitter(this AngoraUser user)
        {
            return user.Logins.Any(l => "Twitter".Equals(l.LoginProvider, StringComparison.OrdinalIgnoreCase));
        }

    }
}
