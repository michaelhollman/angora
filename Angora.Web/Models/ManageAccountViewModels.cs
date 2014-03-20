using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Angora.Data.Models;

namespace Angora.Web.Models
{
    public class ManageAccountViewModel
    {
        public ManageAccountViewModel()
        {
            Errors = new List<string>();
            Successes = new List<string>();
            Infos = new List<string>();
        }

        public AngoraUser User { get; set; }

        public List<string> Errors { get; set; }
        public List<string> Successes { get; set; }
        public List<string> Infos { get; set; }
    }
}