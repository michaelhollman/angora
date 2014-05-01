using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data;
using Angora.Data.Models;

namespace Angora.Services
{
    public class RSVPService : ServiceBase, IRSVPService
    {
        private IRepository<RSVP> _rsvpRepo;
        private IEventService _eventService;

        public RSVPService(IRepository<RSVP> rsvpRepo, IEventService eventService)
        {
            _rsvpRepo = rsvpRepo;
            _eventService = eventService;
        }

        public RSVP Create(RSVP rsvp)
        {
            return _rsvpRepo.Insert(rsvp);
        }

        public RSVP FindById(long id)
        {
            return _rsvpRepo.GetById(id);
        }

        public IEnumerable<RSVP> FindRSVPsForEvent(long eventId)
        {
            return _rsvpRepo.Find(p => p.EventId == eventId);
        }

        public void AddOrUpdateRSVPToEvent(long eventId, RSVP rsvp)
        {
            var vent = _eventService.FindById(eventId);
            vent.RSVPs = vent.RSVPs ?? new List<RSVP>();

            if (rsvp.Id == 0)
            {
                rsvp = Create(rsvp);
                vent.RSVPs.Add(rsvp);
            }
            else
            {
                _rsvpRepo.Update(rsvp);
            }

            _eventService.Update(vent);
        }
    }
}
