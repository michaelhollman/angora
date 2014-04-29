using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class Post : BaseModel
    {
        public virtual AngoraUser User { get; set; }
        public virtual MediaItem MediaItem { get; set; }
        public DateTime PostTime { get; set; }
        public string PostText { get; set; }
        public long EventId { get; set; }
    }
}
