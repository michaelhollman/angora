using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class MediaItem : BaseModel
    {
        public string FooCDNBlob { get; set; }
        public ulong Size { get; set; }
        public MediaType MediaType { get; set; }
    }

    public enum MediaType
    {
        Photo = 1,
        Video = 2,
        Other = -1
    }

    public static class MediaItemExtensions
    {
        public static string GetUrl(this MediaItem media)
        {
            return string.Format("http://foocdn.azurewebsites.net/api/content/{0}", media.FooCDNBlob);
        }
    }
}
