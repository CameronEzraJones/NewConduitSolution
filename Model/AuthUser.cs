using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model
{
    public class AuthUser
    {
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public AuthUser(string username, string email, string password)
        {
            Username = username;
            Email = email;
            Password = password;
        }

        public IdentityUser CreateIdentityUser()
        {
            var user = new IdentityUser();
            user.UserName = Username;
            user.Email = Email;
            return user;
        }
    }
}
