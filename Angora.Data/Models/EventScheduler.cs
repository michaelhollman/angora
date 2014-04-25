using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class EventScheduler : BaseModel
    {
        public bool IsTimeSet { get; set; }
        public virtual List<EventTime> ProposedTimes { get; set; }
        public virtual List<EventSchedulerResponse> Responses { get; set; }
        public virtual List<Tag> Tags { get; set; }
    }
}
