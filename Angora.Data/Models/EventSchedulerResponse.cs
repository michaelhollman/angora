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
        public virtual DateTime Time { get; set; }
        public virtual SchedulerResponse Response { get; set; }
    }

    public enum SchedulerResponse
    {
        Yes = 1,
        No = 2,
        Maybe = 3,
        NoResponse = 0
    }
}
