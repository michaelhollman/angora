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
        public MediaType MediaType { get; set; }
        public ulong Size { get; set; }
    }

    public enum MediaType
    {
        Photo = 1,
        Video = 2,
        Other = -1
    }
}
