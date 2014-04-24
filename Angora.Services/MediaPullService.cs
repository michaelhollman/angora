using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data.Models;
using Facebook;
using TweetSharp;
using Newtonsoft;
using Newtonsoft.Json;

namespace Angora.Services
{
    class MediaPullService : ServiceBase, IMediaPullService
    {
        public void PullFromFacebook(string accessToken, Event theEvent)
        {
            var user = new FacebookClient(accessToken);
            int since = ConvertToTimestamp(theEvent.StartDateTime);
            int until = ConvertToTimestamp(theEvent.EndDateTime);
            JsonObject feed;
            JsonArray data;
            int count = 1; // makes it enter the loop the first time
            JsonObject post;
            JsonObject photo;
            DateTime oldest = new DateTime();
            while (count > 0)
            {
                feed = (JsonObject)user.Get(String.Format("/me/feed?since={0}&until={1}", since, until));
                data = (JsonArray)feed[0];
                count = data.Count;
                Console.WriteLine("Looping\n\n");

                for (int i = 0; i < count; i++)
                {
                    post = (JsonObject)data[i];
                    if ("photo".Equals(post["type"]))
                    {
                        if (post.ContainsKey("status_type"))
                        {
                            photo = (JsonObject)user.Get("/" + post["object_id"]);
                            if (photo.ContainsKey("place"))
                            {
                                var place = (JsonObject)photo["place"];
                                var loc = (JsonObject)place["location"];
                                var lat = loc["latitude"];
                                var lon = loc["longitude"];
                            }
                        }
                    }
                    oldest = DateTime.Parse((string)post["created_time"]);
                }

                until = ConvertToTimestamp(oldest) - 1;
            }
        }

        public void PullFromTwitter(string accessToken, string accessSecret, Event theEvent){
            var service = new TwitterService("o8QTwfzt6CdfDGndyqvLrg", "jqU2tq5QVUkK6JdFA22wtXZNrTumatvG9VpPAfK5M", accessToken, accessSecret);
            var tweets = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions{Count = 10});
            bool keepGoing = true;
            long max = 0;
            do
            {
                if (tweets != null) { 
                    foreach (var tweet in tweets)
                    {
                        DateTime tweetTime = tweet.CreatedDate.ToLocalTime();
                        keepGoing = tweetTime > theEvent.StartDateTime;
                        if (keepGoing && theEvent.EndDateTime > tweetTime)
                        {
                            var name = tweet.User.Name;
                            var text = tweet.Text;
                        }
                        max = tweet.Id - 1;
                    }
                    tweets = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions { ScreenName = "Rayho_1", Count = 10, MaxId = max });
                }
                else
                {
                    keepGoing = false;
                }
            } while (keepGoing);
        }

        private int ConvertToTimestamp(DateTime date)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (int)(date.ToUniversalTime() - epoch).TotalSeconds;
        }

    }
}
