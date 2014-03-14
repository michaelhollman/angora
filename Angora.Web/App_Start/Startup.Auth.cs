using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using System.Threading.Tasks;
using System.Security.Claims;
using Owin;

namespace Angora.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            app.UseTwitterAuthentication(
               consumerKey: "o8QTwfzt6CdfDGndyqvLrg",
               consumerSecret: "jqU2tq5QVUkK6JdFA22wtXZNrTumatvG9VpPAfK5M");

            var facebookOptions = new FacebookAuthenticationOptions();
            facebookOptions.AppId = "1440310966205344";
            facebookOptions.AppSecret = "0ba27f5ec1bcf335fcdf36dc19e71f86";
            facebookOptions.Provider = new FacebookAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                    {
                        context.Identity.AddClaim(new Claim("FacebookAccessToken", context.AccessToken));
                    }
            };
            //we can add scope parameters here
            facebookOptions.Scope.Add("email");
            facebookOptions.Scope.Add("user_birthday");
            facebookOptions.Scope.Add("publish_stream");

            app.UseFacebookAuthentication(facebookOptions);

            //app.UseFacebookAuthentication(
            //   appId: "1440310966205344",
            //   appSecret: "0ba27f5ec1bcf335fcdf36dc19e71f86");

            //app.UseGoogleAuthentication();
        }
    }
}