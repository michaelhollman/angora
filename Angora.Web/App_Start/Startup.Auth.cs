using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using System.Threading.Tasks;
using System.Security.Claims;
using Owin;
using Microsoft.Owin.Security.Twitter;

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
                LoginPath = new PathString("/login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //Twitter
            var twitterOptions = new TwitterAuthenticationOptions();
            twitterOptions.ConsumerKey = "o8QTwfzt6CdfDGndyqvLrg";
            twitterOptions.ConsumerSecret = "jqU2tq5QVUkK6JdFA22wtXZNrTumatvG9VpPAfK5M";
            twitterOptions.Provider = new TwitterAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                    {
                        context.Identity.AddClaim(new Claim("TwitterAccessToken", context.AccessToken));
                        context.Identity.AddClaim(new Claim("TwitterAccessSecret", context.AccessTokenSecret));
                    }
            };

            app.UseTwitterAuthentication(twitterOptions);
            
            //Facebook
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
        }
    }
}