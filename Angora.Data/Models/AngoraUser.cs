using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;


namespace Angora.Data.Models
{
    public class AngoraUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Location { get; set; }
        public DateTime? Birthday { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string FacebookAccessToken { get; set; }
        public string TwitterAccessToken { get; set; }
        public string TwitterAccessSecret { get; set; }

        // TODO actually have lots of real non-placeholder user information in here

        // TODO research if this needs to be changed structurally for having multiple services associated with one account
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
