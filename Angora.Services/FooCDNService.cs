using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

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

        public async Task PostToBlob(string blobID, byte[] bytes, string filename)
        {
            string url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}", blobID);
            var requestContent = new MultipartFormDataContent();
            var image = new ByteArrayContent(bytes);
            var extension = Path.GetExtension(filename).TrimStart('.');
            filename = Path.GetFileName(filename);
            image.Headers.ContentType = MediaTypeHeaderValue.Parse(string.Format("image/{0}", extension));

            requestContent.Add(image, "data", filename);

            var client = new HttpClient();
            await client.PostAsync(url, requestContent);
        }

        public void DeleteBlob(string blobID)
        {
            var url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}", blobID);
            var request = WebRequest.Create(url);
            request.Method = "DELETE";
            request.ContentLength = 0;
            var response = request.GetResponse();
        }

        public string GetBlobInfo(string blobID)
        {
            var url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}/info", blobID);
            var request = WebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            var response = request.GetResponse();
            var info = "";
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                info = reader.ReadToEnd();
            }
            return info;
        }

        //Not sure if we need this
        public void GetBlob(string blobID, string fileName)
        {
            var url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}", blobID);
            var client = new WebClient();
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
                var url = String.Format("http://foocdn.azurewebsites.net/api/content/{0}?type={1}", blobID, destination);
                var request = WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentLength = 0;
                var response = request.GetResponse();
            }
        }

        public string CreateNewBlob(string mimeType)
        {
            var json = JsonConvert.SerializeObject(new { AccountKey = "CFE39AC9FE8AB", MimeType = mimeType });
            var request = (HttpWebRequest)WebRequest.Create("http://foocdn.azurewebsites.net/api/content/add");
            request.Method = "POST";
            request.ContentType = "text/json";
            var dataStream = new StreamWriter(request.GetRequestStream());
            dataStream.Write(json);
            dataStream.Close();
            var response = request.GetResponse();
            var blobId = "";
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                blobId = reader.ReadToEnd();
            }
            // get rid of the quotes
            return blobId.Substring(1, blobId.Length - 2);
        }
    }
}
