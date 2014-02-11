using System;
using System.ComponentModel.DataAnnotations;

namespace Angora.Data
{
    public abstract class BaseModel
    {
        [Key]
        public long Id { get; set; }
    }
}
