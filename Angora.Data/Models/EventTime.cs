using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class EventTime : BaseModel
    {
        private DateTime _startTime { get; set; }
        public DateTime StartTime
        {
            get
            {
                return DateTime.SpecifyKind(_startTime, DateTimeKind.Utc);
            }
            set
            {
                _startTime = value;
            }
        }
        public int DurationInMinutes { get; set; }
    }

    public static class EventTimeExtensions
    {
        public static EventTime AsEventTime(this DateTime dt)
        {
            return new EventTime
            {
                StartTime = dt,
                DurationInMinutes = 0
            };
        }

        public static DateTime GetEndTime(this EventTime et)
        {
            return et.StartTime.AddMinutes(et.DurationInMinutes);
        }
    }
}
