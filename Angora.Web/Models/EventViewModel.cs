using Angora.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Angora.Web.Models
{
    public class NewEventViewModel
    {
        public NewEventViewModel()
        {
            ScheduleNow = true;
            StartDateTime = DateTime.Today.AddDays(1);
        }

        [Required]
        [Display(Name = "Event Name")]
        //did we want to have something like this?
        [StringLength(100, ErrorMessage = "The event name is too long.")]
        public string Name { get; set; }

        [Display(Name = "Event Description")]
        public string Description { get; set; }

        [Display(Name = "LocationStr")]
        //latitude then longitude
        public string Location { get; set; }

        [Display(Name = "Latitude")]
        public string Latitude { get; set; }

        [Display(Name = "Longitude")]
        public string Longitude { get; set; }

        public bool ScheduleNow { get; set; }

        [Display(Name = "Start Date and Time")]
        public DateTime StartDateTime { get; set; }

        public int DurationHours { get; set; }
        public int DurationMinutes { get; set; }

        //public List<Tag> Tags { get; set; }
    }

    public class EventEditViewModel
    {
        public Event Event { get; set; }
        public int DurationHours { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class EventViewModel
    {
        public Event Event { get; set; }
        public AngoraUser Viewer { get; set; }
        public bool ViewerIsCreator { get; set; }
        public int DurationHours { get { return Event.EventTime.DurationInMinutes / 60; } }
        public int DurationMinutes { get { return Event.EventTime.DurationInMinutes % 60; } }
        public DateTime EndTime { get { return Event.EventTime.GetEndTime(); } }
    }
}