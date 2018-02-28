using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model
{
    public class UserUpdateData
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [Url]
        public string Image { get; set; }

        public string Bio { get; set; }
    }
}
