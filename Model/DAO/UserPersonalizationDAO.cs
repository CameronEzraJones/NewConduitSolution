using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.DAO
{
    public class UserPersonalizationDAO
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public string Bio { get; set; }

        [Url]
        public string Image { get; set; }

        public IdentityUser User { get; set; }

        public UserPersonalizationDAO()
        {

        }

        public UserPersonalizationDAO(
            string userId,
            string bio,
            string image
            )
        {
            UserId = userId;
            Bio = bio;
            Image = image;
        }
    }
}
