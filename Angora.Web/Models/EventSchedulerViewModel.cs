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
        public bool ViewerOwnsEvent { get; set; }

    }
}