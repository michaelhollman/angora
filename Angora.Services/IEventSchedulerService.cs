using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data.Models;

namespace Angora.Services
{
    public interface IEventSchedulerService
    {
        void SetResponse(long eventId, string userId, SchedulerResponse response);

    }
}
