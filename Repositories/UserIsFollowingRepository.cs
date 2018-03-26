using Conduit.Context;
using Conduit.Model.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Conduit.Repositories
{
    public class UserIsFollowingRepository : IUserIsFollowingRepository
    {
        private readonly DbSet<UserIsFollowingDAO> _userIsFollowingDAO;

        public UserIsFollowingRepository(ConduitDbContext context)
        {
            _userIsFollowingDAO = context.UserIsFollowing;
        }

        public override void FollowUser(string followerId, string followeeId)
        {
            UserIsFollowingDAO userIsFollowing = new UserIsFollowingDAO
            {
                FollowerId = followerId,
                FolloweeId = followeeId
            };
            _userIsFollowingDAO.Add(userIsFollowing);
        }

        public override List<string> GetListOfFollowees(string followerId)
        {
            return _userIsFollowingDAO.Where(e => e.FollowerId == followerId).Select(e => e.FolloweeId).ToList();
        }

        public override bool IsUserFollowing(string followerId, string followeeId)
        {
            UserIsFollowingDAO userIsFollowing = _userIsFollowingDAO.Where(e => e.FollowerId == followerId && e.FolloweeId == followeeId).SingleOrDefault();
            return null != userIsFollowing ? true : false;
        }

        public override void UnfollowUser(string followerId, string followeeId)
        {
            UserIsFollowingDAO userIsFollowing = _userIsFollowingDAO.Where(e => e.FollowerId == followerId && e.FolloweeId == followeeId).Single();
            _userIsFollowingDAO.Remove(userIsFollowing);
        }
    }
}
