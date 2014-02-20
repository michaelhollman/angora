using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Angora.Data.Models
{
    public class AngoraUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // TODO actually have lots of real non-placeholder user information in here

        // TODO research if this needs to be changed structurally for having multiple services associated with one account
    }
}
