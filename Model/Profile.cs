using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model
{
    public class Profile
    {
        [Required]
        public string Username { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        public bool Following { get; set; }

        public Profile()
        {

        }

        public Profile(string username, string bio, string image, bool following)
        {
            Username = username;
            Bio = bio;
            Image = image;
            Following = following;
        }
    }
}
