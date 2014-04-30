using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data.Models;

namespace Angora.Services
{
    public interface IRSVPService
    {
        RSVP Create(RSVP post);
        RSVP FindById(long id);
        IEnumerable<RSVP> FindRSVPsForEvent(long eventId);
        void AddOrUpdateRSVPToEvent(long eventId, RSVP RSVP);
    }
}
