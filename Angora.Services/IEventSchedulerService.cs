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
        void SetResponse(long eventId, AngoraUser user, SchedulerResponse response, DateTime time);
        void AddProposedTimeToEvent(long eventId, EventTime evTime);
        void FinalizeTime(long eventId, DateTime time);
    }
}
