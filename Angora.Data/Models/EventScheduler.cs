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
        public List<EventTime> ProposedTimes { get; set; }
        public List<EventSchedulerResponse> Responses { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
