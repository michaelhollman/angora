﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data.Models;

namespace Angora.Services
{
    public interface IMediaPullService
    {
        void PullFromFacebook(string accessToken, Event theEvent);
        void PullFromTwitter(string accessToken, string accessSecret, Event theEvent);

    }
}