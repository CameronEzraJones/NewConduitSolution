using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.DAO
{
    public class UserIsFollowingDAO
    {
        public UserIsFollowingDAO()
        {

        }

        public UserIsFollowingDAO(string followerId, string followeeId)
        {
            FollowerId = followerId;
            FolloweeId = followeeId;
        }

        public IdentityUser Follower { get; set; }
        public IdentityUser Followee { get; set; }

        [ForeignKey("Follower")]
        public string FollowerId { get; set; }

        [ForeignKey("Followee")]
        public string FolloweeId { get; set; }
    }
}
