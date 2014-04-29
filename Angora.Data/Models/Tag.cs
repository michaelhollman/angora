using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angora.Data.Models
{
    public class Tag : BaseModel
    {
        String Text { get; set; }

        public Tag()
        {
            Text = string.Empty;
        }

        public Tag(string tag)
        {
            Text = tag;
        }
    }

    public static class TagExtensions
    {
        public static Tag ToTag(this string str)
        {
            return new Tag(str);
        }
    }
}
