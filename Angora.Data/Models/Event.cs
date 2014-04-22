using System;
using System.Collections.Generic;

namespace Angora.Data.Models
{
    public class Event : BaseModel
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //formatted address
        public string Location { get; set; }
        //lat then lng
        public string Coordinates { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string Tags { get; set; }

        public DateTime CreationTime { get; set; }

    }
}
