﻿using System;
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
            IsFirstTimeLogin = false;
        }

        public AngoraUser User { get; set; }

        public bool IsFirstTimeLogin { get; set; }

        public string FacebookPic { get; set; }
        public string TwitterPic { get; set; }

        public List<string> Errors { get; set; }
        public List<string> Successes { get; set; }
        public List<string> Infos { get; set; }
    }

    public class ReturnUrlViewModel
    {
        public string ReturnUrl { get; set; }
    }
}