using Conduit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Services
{
    public abstract class IProfileService
    {
        public abstract Task<Profile> GetProfile(String username, String authedUsername = null);
        public abstract Task<Profile> SetUserIsFollowing(String username, String authedUsername, Boolean isFollowing);
    }
}
