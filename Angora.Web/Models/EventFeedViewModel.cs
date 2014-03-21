using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Angora.Data.Models;
using Angora.Services;


namespace Angora.Web.Models
{
    public class EventFeedViewModel
    {
        public IEnumerable<Event> Events { get; set; }
    }
}