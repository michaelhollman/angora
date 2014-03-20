using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class Event : BaseModel
    {
        public long UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //latitude then longitude
        public string Location { get; set; }

        public DateTime Time { get; set; }

        public List<string> Tags { get; set; }

        public DateTime CreationTime { get; set; }

    }
}
