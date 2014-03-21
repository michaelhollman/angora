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
        [Required]
        [Display(Name= "Event Name")]
        //did we want to have something like this?
        [StringLength(100,ErrorMessage="The event name is too long.")]
        public string Name { get; set; }
       
        [Display(Name = "Event Description")]
        public string Description { get; set; }

        [Display(Name = "Location")]
        //latitude then longitude
        public string Location { get; set; }

        [Display(Name = "Start Date and StartDateTime")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "End Date and StartDateTime")]
        public DateTime EndDateTime { get; set; }

        [Display(Name= "Tags")]
        public List<string> Tags { get; set; }
    }

    public class EditEventViewModel
    {
        public long EventId { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        //did we want to have something like this?
        [StringLength(100, ErrorMessage = "The event name is too long.")]
        public string Name { get; set; }

        [Display(Name = "Event Description")]
        public string Description { get; set; }

        [Display(Name = "Location")]
        //latitude then longitude
        public string Location { get; set; }

        [Display(Name = "Start Date and StartDateTime")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "End Date and StartDateTime")]
        public DateTime EndDateTime { get; set; }

        [Display(Name = "Tags")]
        public List<string> Tags { get; set; }
    }

}