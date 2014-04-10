using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Angora.Data.Models;
using Angora.Services;
using Angora.Web.Models;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using TweetSharp;

namespace Angora.Web.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : Controller
    {
        
        public IAngoraUserService _userService;

        public AccountController(IAngoraUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [Route("~/login")]
        public ActionResult Login(string returnUrl)
        {
            return View("Login", new ReturnUrlViewModel { ReturnUrl = returnUrl });
        }

        [HttpGet]
        [Route("~/logout")]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region External Logins

        [AllowAnonymous]
        [Route("externallogin")]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return ExternalLoginRequest(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }), provider);
        }


        [Route("externallogin/callback")]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                return RedirectToAction("Login", new { returnUrl });
            }

            var addingFacebook = "Facebook".Equals(loginInfo.Login.LoginProvider, StringComparison.OrdinalIgnoreCase);
            var addingTwitter = "Twitter".Equals(loginInfo.Login.LoginProvider, StringComparison.OrdinalIgnoreCase);

            // Sign in the user with this external login provider if the user already has a login
            var user = _userService.FindUser(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user);
                return Redirect(returnUrl ?? Url.Action("Index", "Home"));
            }

            if (addingFacebook)
            {
                ClaimsIdentity externalCookie = await HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                var accessToken = GetExtendedFacebookAccessToken(externalCookie.Claims.First(x => x.Type.Contains("FacebookAccessToken")).Value);
                dynamic facebookUser = new FacebookClient(accessToken).Get(loginInfo.Login.ProviderKey);

                user = new AngoraUser
                {
                    FacebookAccessToken = accessToken,
                    FirstName = facebookUser.first_name,
                    LastName = facebookUser.last_name,
                    EmailAddress = facebookUser.email,
                    Location = facebookUser.location.name,
                    Birthday = Convert.ToDateTime(facebookUser.birthday)
                };
            }
            else if (addingTwitter)
            {
                ClaimsIdentity externalCookie = await HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                var accessToken = externalCookie.Claims.First(x => x.Type.Contains("TwitterAccessToken")).Value;
                var accessSecret = externalCookie.Claims.First(x => x.Type.Contains("TwitterAccessSecret")).Value;

                var twitterService = new TwitterService("o8QTwfzt6CdfDGndyqvLrg", "jqU2tq5QVUkK6JdFA22wtXZNrTumatvG9VpPAfK5M", accessToken, accessSecret);
                var twitterUser = twitterService.GetUserProfile(new GetUserProfileOptions());

                string firstName, lastName;
                if (twitterUser.Name.Contains(' '))
                {
                    var names = twitterUser.Name.Split(' ');
                    firstName = names[0];
                    lastName = names[1];
                }
                else
                {
                    firstName = twitterUser.Name;
                    lastName = "";
                }

                user = new AngoraUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Location = twitterUser.Location,
                    TwitterAccessToken = accessToken,
                    TwitterAccessSecret = accessSecret,
                };
            }
            else
            {
                //if this happens, something went very wrong
                //login provider isn't facebook or twitter
                return View("Login", new ReturnUrlViewModel { ReturnUrl = returnUrl });
            }

            //TODO, we need a better solution for usernames, or a convenient way to make them irrelevant.
            // we only really use it in the navbar, so perhaps some sort of global model?
            // viewbag would be perfect, but gross.
            user.UserName = user.Id;

            var create = await _userService.CreateUser(user);
            if (create.Succeeded)
            {
                var add = await _userService.AddLogin(user.Id, loginInfo.Login);
                if (add.Succeeded)
                {
                    await SignInAsync(user);

                    var model = new ManageAccountViewModel();
                    model.User = user;
                    model.Successes.Add("Welcome to Angora! Please fill out your information.");
                    model.IsFirstTimeLogin = true;
                    return View("Index", model);
                }
            }
            // TODO .... uhhhhh
            return new EmptyResult();
        }


        [Authorize]
        [Route("exterallogin/add")]
        public ActionResult AddExternalLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return ExternalLoginRequest(Url.Action("AddExternalLoginCallback"), provider);
        }

        [Authorize]
        [Route("externallogin/add/callback")]
        public async Task<ActionResult> AddExternalLoginCallback()
        {
            // TODO this completely shits itself when we try to add a login that already has an account...

            var model = new ManageAccountViewModel();

            var loginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                model.Errors.Add("Uhoh... something didn't quite go right with that. Sorry.");
                return View("Index", model);
            }

            var result = await _userService.AddLogin(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                model.User = await _userService.FindUserById(User.Identity.GetUserId());
                model.Successes.Add(string.Format("Successfully added your {0} account!", loginInfo.Login.LoginProvider));
                return View("Index", model);
            }

            model.Errors.Add("Uhoh... something didn't quite go right with that. Sorry.");
            return View("Index", model);
        }

        [Authorize]
        [Route("externallogin/remove")]
        public async Task<ActionResult> RemoveExternalLogin(string loginProvider)
        {
            var model = new ManageAccountViewModel();
            var user = await _userService.FindUserById(User.Identity.GetUserId());
            var remove = user.Logins.First(l => l.LoginProvider.Equals(loginProvider, StringComparison.OrdinalIgnoreCase));

            IdentityResult result = await _userService.RemoveLogin(User.Identity.GetUserId(), new UserLoginInfo(remove.LoginProvider, remove.ProviderKey));
            if (result.Succeeded)
            {
                model.Successes.Add(string.Format("Succesfully unlinked {0} account.", remove.LoginProvider));
            }
            else
            {
                model.Errors.Add("Uhoh, something didn't quite go right with that. Sorry.");
            }

            model.User = await _userService.FindUserById(User.Identity.GetUserId());
            return View("Index", model);
        }


        private ActionResult ExternalLoginRequest(string redirectUri, string provider)
        {
            // Request a redirect to the external login provider
            var ctx = Request.GetOwinContext();
            ctx.Authentication.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = redirectUri
                },
                provider);
            return new HttpUnauthorizedResult();
        }

        private async Task SignInAsync(AngoraUser user)
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userService.CreateIdentity(user);
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
        }

        private string GetExtendedFacebookAccessToken(string shortLivedToken)
        {
            FacebookClient client = new FacebookClient();
            string extendedToken = "";
            try
            {
                dynamic result = client.Get("/oauth/access_token", new
                {
                    grant_type = "fb_exchange_token",
                    client_id = "1440310966205344",
                    client_secret = "0ba27f5ec1bcf335fcdf36dc19e71f86",
                    fb_exchange_token = shortLivedToken
                });
                extendedToken = result.access_token;
            }
            catch
            {
                extendedToken = shortLivedToken;
            }
            return extendedToken;
        }

        #endregion
        #region Normal Management

        [HttpGet]
        [Authorize]
        [Route("")]
        public async Task<ActionResult> Index(ManageAccountViewModel param = null)
        {
            var model = param ?? new ManageAccountViewModel();
            model.User = await _userService.FindUserById(User.Identity.GetUserId());
            return View("Index", model);
        }


        [HttpPost]
        [Authorize]
        [Route("update")]
        public async Task<ActionResult> UpdateUserInfo(ManageAccountViewModel param)
        {
            var model = new ManageAccountViewModel();

            var user = await _userService.FindUserById(User.Identity.GetUserId());
            user.FirstName = param.User.FirstName ?? user.FirstName;
            user.LastName = param.User.LastName ?? user.LastName;
            user.EmailAddress = param.User.EmailAddress ?? user.EmailAddress;
            user.Location = param.User.Location ?? user.Location;
            user.Birthday = param.User.Birthday != null ? param.User.Birthday : user.Birthday;

            var update = await _userService.UpdateUser(user);
            if (update.Succeeded)
            {
                model.Successes.Add("Successfully updated account");
            }
            else
            {
                model.Errors.Add("There was an error trying to update your account");
                model.Errors.AddRange(update.Errors);
            }

            return await Index(model);
        }

        #endregion
    }
}