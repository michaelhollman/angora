using System;
using System.Collections.Generic;

namespace Angora.Data.Models
{
    public class Event : BaseModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //latitude then longitude
        public string Location { get; set; }

        // TODO replace with EventTime object
        public DateTime StartDateTime { get; set; }

        // TODO phase this out (replacing with duration on EventTime)

        public DateTime EndDateTime { get; set; }
        public List<Tag> Tags { get; set; }
        public DateTime CreationTime { get; set; }

        // Scheduler stuff

        public EventScheduler Scheduler { get; set; }

    }
}
