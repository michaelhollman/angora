using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class RSVP : BaseModel
    {
        public virtual AngoraUser User { get; set; }
        public RSVPStatus Response { get; set; }
        public long EventId { get; set; }
    }

    public enum RSVPStatus
    {
        Yes = 1,
        No = 2,
        Maybe = 3,
        NoResponse = 0
    }
}
