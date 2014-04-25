﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class EventSchedulerResponse : BaseModel
    {
        public virtual AngoraUser User { get; set; }
        public virtual Dictionary<EventTime, SchedulerResponse> Responses { get; set; }
    }

    public enum SchedulerResponse
    {
        No = 0,
        Yes = 1,
        Maybe = 3,
        //NoResponse = -1
    }
}
