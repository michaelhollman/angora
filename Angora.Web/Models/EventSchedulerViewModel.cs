using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Angora.Data.Models;

namespace Angora.Web.Models
{
    public class EventSchedulerViewModel
    {
        public Event Event { get; set; }
        public bool ViewerIsCreator { get; set; }
        public List<EventSchedulerProposedTimeViewModel> Times { get; set; }

    }

    public class EventSchedulerProposedTimeViewModel
    {
        public DateTime Time { get; set; }
        public SchedulerResponse CurrentUserResponse { get; set; }
        public int YesCount { get; set; }
        public int MaybeCount { get; set; }
        public int NoCount { get; set; }
    }
}