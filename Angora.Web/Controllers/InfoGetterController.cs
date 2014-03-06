using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Angora.Data;
using Angora.Data.Models;
using Angora.Web.Models;
using Facebook;

namespace Angora.Web.Controllers
{
    public class InfoGetterController : Controller 
    {
        public InfoGetterController()
            : this(new UserManager<AngoraUser>(new UserStore<AngoraUser>(new AngoraDbContext())))
        {
        }

        public InfoGetterController(UserManager<AngoraUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<AngoraUser> UserManager { get; private set; }
        
        public ActionResult Index()
        {
            var accessToken = "1440310966205344|CSK33sTmDVY4XRuyAuWv286IFp4";

            InfoGetterModel model = new InfoGetterModel();

            if (User.Identity.IsAuthenticated)
            {
                IList<UserLoginInfo> logins = UserManager.GetLogins(User.Identity.GetUserId());
                var client = new FacebookClient(accessToken);
                dynamic user = null;

                foreach (UserLoginInfo login in logins)
                {
                    if (login.LoginProvider == "Facebook")
                    {
                        model.UserID = login.ProviderKey;
                        user = client.Get(login.ProviderKey);
                    }
                }

                model.FirstName = user.first_name;
                model.LastName = user.last_name;
                model.Gender = user.gender;
                model.Location = user.location.name;
                model.Link = user.link;
                model.PictureUrl = "https://graph.facebook.com/" + user.id + "/picture";

                dynamic friendsListData = client.Get("/" + model.UserID + "/friends");
                model.FriendsList = new List<Friend>();
                foreach (var friend in friendsListData.data)
                {
                    model.FriendsList.Add(new Friend { Name = friend.name, Id = friend.id, PictureUrl = "https://graph.facebook.com/"+friend.id+"/picture" });
                    
                }
            }
            


            return View(model);
        }
    }
}