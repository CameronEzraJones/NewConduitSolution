using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public abstract class IUserIsFollowingRepository
    {
        public abstract Boolean IsUserFollowing(String followerId, String followeeId);
        public abstract void FollowUser(String followerId, String followeeId);
        public abstract void UnfollowUser(String followerId, String followeeId);
        public abstract List<String> GetListOfFollowees(String followerId);
    }
}
