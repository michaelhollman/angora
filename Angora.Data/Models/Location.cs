using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class Location : BaseModel
    {
        public string NameOrAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
