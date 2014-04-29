using System;
using System.Collections.Generic;

namespace Angora.Data.Models
{
    public class Event : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual AngoraUser Creator { get; set; }
        public virtual Location Location { get; set; }
        public virtual EventTime EventTime { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public DateTime CreationTime { get; set; }
        public virtual EventScheduler Scheduler { get; set; }
        public virtual List<Post> Posts { get; set; }

    }
}
