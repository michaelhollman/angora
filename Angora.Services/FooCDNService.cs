using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Angora.Services
{
    class FooCDNService : ServiceBase, IFooCDNService
    {
        ////////To Do createNewBlob(string mimeType)////////////////

        public void postToBlob(string blobID, string filename)
        {
            StringBuilder url = new StringBuilder("http://foocdn.azurewebsites.net/api/content/");
            url.Append(blobID);
            WebClient client = new WebClient();
            client.UploadFile(url.ToString(), filename);
        }

        public void deleteBlob(string blobID)
        {
            StringBuilder url = new StringBuilder("http://foocdn.azurewebsites.net/api/content/");
            url.Append(blobID);
            WebRequest request = WebRequest.Create(url.ToString());
            request.Method = "DELETE";
            request.ContentLength = 0;
            WebResponse response = request.GetResponse();
        }

        public string getBlobInfo(string blobID)
        {
            StringBuilder url = new StringBuilder("http://foocdn.azurewebsites.net/api/content/");
            url.Append(blobID);
            url.Append("/info");
            //WebClient client = new WebClient();
            //return client.DownloadString(url.ToString());
            WebRequest request = WebRequest.Create(url.ToString());
            request.Method = "GET";
            request.ContentLength = 0;
            WebResponse response = request.GetResponse();
            string info = "";
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                info = reader.ReadToEnd();
            }
            return info;
        }

        //Not sure if we need this
        public void getBlob(string blobID, string fileName)
        {
            StringBuilder url = new StringBuilder("http://foocdn.azurewebsites.net/api/content/");
            url.Append(blobID);
            WebClient client = new WebClient();
            client.DownloadFile(url.ToString(), fileName);
        }

        public string getBlobURL(string blobID)
        {
            StringBuilder url = new StringBuilder("http://foocdn.azurewebsites.net/api/content/");
            url.Append(blobID);
            return url.ToString();
        }

        public void putBlob(string blobID, string destination)
        {
            destination = destination.ToUpper();
            if (!(destination.Equals("MEMCACHE") || destination.Equals("DISK") || destination.Equals("TAPE")))
            {
                throw new Exception("destination was not valid");
            }
            else
            {
                StringBuilder url = new StringBuilder("http://foocdn.azurewebsites.net/api/content/");
                url.Append(blobID);
                url.Append("?type=");
                url.Append(destination);
                WebRequest request = WebRequest.Create(url.ToString());
                request.Method = "PUT";
                request.ContentLength = 0;
                WebResponse response = request.GetResponse();
            }
        }

        public string createNewBlob(string mimeType)
        {
            string json = JsonConvert.SerializeObject(new { AccountKey = "CFE39AC9FE8AB", MimeType = mimeType });
            var request = (HttpWebRequest)WebRequest.Create("http://foocdn.azurewebsites.net/api/content/add");
            request.Method = "POST";
            request.ContentType = "text/json";
            StreamWriter dataStream = new StreamWriter(request.GetRequestStream());
            dataStream.Write(json);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            string blobId = "";
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                blobId = reader.ReadToEnd();
            }
            // get rid of the quotes
            return blobId.Substring(1, blobId.Length - 2);
        }
    }
}
