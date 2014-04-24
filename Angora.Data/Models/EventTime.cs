﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class EventTime : BaseModel
    {
        public DateTime Time { get; set; }
        public int? DurationInMinutes { get; set; }
    }

    public static class EventTimeExtensions
    {
        public static EventTime AsEventTime(this DateTime dt)
        {
            return new EventTime
            {
                Time = dt
            };
        }
    }
}