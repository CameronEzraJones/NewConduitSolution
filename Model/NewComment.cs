using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model
{
    public class NewComment
    {
        [Required]
        public string Body { get; set; }
    }
}
