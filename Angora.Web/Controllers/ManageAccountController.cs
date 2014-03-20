using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Angora.Data.Models;
using Angora.Services;
using Angora.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Facebook;
using TweetSharp;

namespace Angora.Web.Controllers
{
    public class ManageAccountController : Controller
    {

        private IAngoraUserService _userService;

        public ManageAccountController()
            : this(ServiceManager.GetService<IAngoraUserService>())
        {
        }

        public ManageAccountController(IAngoraUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(string returnUrl)
        {
            return View("Login", new ReturnUrlViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [Route("account/externallogin")]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            if (!User.Identity.IsAuthenticated)
            {
                var ctx = Request.GetOwinContext();
                ctx.Authentication.Challenge(
                    new AuthenticationProperties
                    {
                        RedirectUri = Url.Action("ExternalLoginCallback", "ManageAccount", new { ReturnUrl = returnUrl })
                    },
                    provider);
                return new HttpUnauthorizedResult();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                return RedirectToAction("Login", new { returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = _userService.FindUser(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user);
                return Redirect(returnUrl ?? "/");
            }
            else
            {
                AngoraUser newUser = null;

                if ("Facebook".Equals(loginInfo.Login.LoginProvider, StringComparison.OrdinalIgnoreCase))
                {
                    //facebook info pulling
                    ClaimsIdentity externalCookie = await HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                    var accessToken = GetExtendedFacebookAccessToken(externalCookie.Claims.First(x => x.Type.Contains("FacebookAccessToken")).Value);
                    dynamic facebookUser = new FacebookClient(accessToken).Get(loginInfo.Login.ProviderKey);

                    newUser = new AngoraUser
                    {
                        FacebookAccessToken = accessToken,
                        FirstName = facebookUser.first_name,
                        LastName = facebookUser.last_name,
                        EmailAddress = facebookUser.email,
                        Location = facebookUser.location.name,
                        Birthday = Convert.ToDateTime(facebookUser.birthday)
                    };
                    //TODO, we need a better solution for usernames, or a convenient way to make them irrelevant.
                    // we only really use it in the navbar, so perhaps some sort of global model?
                    // viewbag would be perfect, but gross.
                    newUser.UserName = newUser.Id;
                }
                else if ("Twitter".Equals(loginInfo.Login.LoginProvider, StringComparison.OrdinalIgnoreCase))
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

                    newUser = new AngoraUser
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Location = twitterUser.Location,
                        TwitterAccessToken = accessToken,
                        TwitterAccessSecret = accessSecret,
                    };
                    //TODO, we need a better solution for usernames, or a convenient way to make them irrelevant.
                    // we only really use it in the navbar, so perhaps some sort of global model?
                    // viewbag would be perfect, but gross.
                    newUser.UserName = newUser.Id;
                }
                else
                {
                    //if this happens, something went very wrong
                    //login provider isn't facebook or twitter
                    return View("Login", new ReturnUrlViewModel { ReturnUrl = returnUrl });
                }

                var create = await _userService.CreateUser(newUser);
                if (create.Succeeded)
                {
                    var add = await _userService.AddLogin(newUser.Id, loginInfo.Login);
                    if (add.Succeeded)
                    {
                        await SignInAsync(newUser);

                        var model = new ManageAccountViewModel();
                        model.User = newUser;
                        model.Successes.Add("Welcome to Angora! Please fill out your information.");
                        model.IsFirstTimeLogin = true;
                        return View("Index", model);
                    }
                }

                // TODO .... uhhhhh
                return new EmptyResult();

            }
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

        [HttpGet]
        [Route("logout")]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        private async Task SignInAsync(AngoraUser user)
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userService.CreateIdentity(user);
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
        }




        [HttpGet]
        [Authorize]
        [Route("account")]
        public async Task<ActionResult> Index(ManageAccountViewModel param = null)
        {
            var model = param ?? new ManageAccountViewModel();
            model.User = await _userService.FindUserById(User.Identity.GetUserId());
            return View("Index", model);
        }


        [HttpPost]
        [Authorize]
        [Route("account/update")]
        public async Task<ActionResult> UpdateUserInfo(ManageAccountViewModel param)
        {
            var model = new ManageAccountViewModel();

            var user = await _userService.FindUserById(User.Identity.GetUserId());
            user.FirstName = param.User.FirstName ?? user.FirstName;
            user.LastName = param.User.LastName ?? user.LastName;
            user.EmailAddress = param.User.EmailAddress ?? user.EmailAddress;
            user.Location = param.User.Location ?? user.Location;
            user.Birthday = param.User.Birthday != null ? param.User.Birthday : user.Birthday;
            user.UserName = String.Format("{0} {1}", user.FirstName, user.LastName);

            var update = await _userService.UpdateUser(user);
            if (update.Succeeded)
            {
                model.Successes.Add("Successfully updated account");
            }
            else
            {
                model.Errors.Add("There was an error trying to update your account");
            }

            return await Index(model);
        }
    }
}