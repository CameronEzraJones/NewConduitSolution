using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model
{
    public class User
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Token { get; set; }

        [Required]
        public string Username { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        public User(
            string email,
            string token,
            string username,
            string bio,
            string image)
        {
            Email = email;
            Token = token;
            Username = username;
            Bio = bio;
            Image = image;
        }
    }
}
