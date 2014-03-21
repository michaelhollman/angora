using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Angora.Services
{
    public class FooCDNService : ServiceBase, IFooCDNService
    {
        public void PostToBlob(string blobID, string filename)
        {
            string url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}", blobID);
            WebClient client = new WebClient();
            client.UploadFile(url, filename);
        }

        public void DeleteBlob(string blobID)
        {
            string url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}", blobID);
            WebRequest request = WebRequest.Create(url);
            request.Method = "DELETE";
            request.ContentLength = 0;
            WebResponse response = request.GetResponse();
        }

        public string GetBlobInfo(string blobID)
        {
            string url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}/info", blobID);
            WebRequest request = WebRequest.Create(url);
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
        public void GetBlob(string blobID, string fileName)
        {
            string url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}", blobID);
            WebClient client = new WebClient();
            client.DownloadFile(url, fileName);
        }

        public string GetBlobURL(string blobID)
        {
            return String.Format("http://foocdn.azurewebsites.net/api/content/{0}", blobID);
        }

        public void PutBlob(string blobID, string destination)
        {
            destination = destination.ToUpper();
            if (!(destination.Equals("MEMCACHE") || destination.Equals("DISK") || destination.Equals("TAPE")))
            {
                throw new Exception("destination was not valid");
            }
            else
            {
                string url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}?type={1}", blobID, destination);
                WebRequest request = WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentLength = 0;
                WebResponse response = request.GetResponse();
            }
        }

        public string CreateNewBlob(string mimeType)
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
