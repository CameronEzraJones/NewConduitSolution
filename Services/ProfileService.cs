using Conduit.Context;
using Conduit.Model;
using Conduit.Model.DAO;
using Conduit.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserPersonalizationRepository _userPersonalizationRepository;
        private readonly IUserIsFollowingRepository _userIsFollowingRepository;
        private readonly ConduitDbContext _context;

        public ProfileService(
            UserManager<IdentityUser> userManager,
            IUserPersonalizationRepository userPersonalizationRepository,
            IUserIsFollowingRepository userIsFollowingRepository,
            ConduitDbContext context)
        {
            _userManager = userManager;
            _userPersonalizationRepository = userPersonalizationRepository;
            _userIsFollowingRepository = userIsFollowingRepository;
            _context = context;
        }

        public override async Task<Profile> GetProfile(String username, String authedUsername = null)
        {
            Profile profile = new Profile(null, null, null, false);
            IdentityUser profileUser = null;
            try
            {
                profileUser = await _userManager.FindByNameAsync(username);
                profile.Username = profileUser.UserName;
                UserPersonalizationDAO userPersonalization = _userPersonalizationRepository.GetUserPersonalization(profileUser.Id);
                profile.Bio = userPersonalization.Bio;
                profile.Image = userPersonalization.Image;
            } catch (Exception ex)
            {
                throw ex;
            }
            if(null != authedUsername)
            {
                try
                {
                    IdentityUser authedUser = await _userManager.FindByNameAsync(authedUsername);
                    string authedUserId = authedUser.Id;
                    profile.Following = _userIsFollowingRepository.IsUserFollowing(authedUserId, profileUser.Id);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return profile;
        }

        public override async Task<Profile> SetUserIsFollowing(string username, string authedUsername, bool isFollowing)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                IdentityUser follower = await _userManager.FindByNameAsync(authedUsername);
                IdentityUser followee = await _userManager.FindByNameAsync(username);
                if(isFollowing)
                {
                    _userIsFollowingRepository.FollowUser(follower.Id, followee.Id);
                } else
                {
                    _userIsFollowingRepository.UnfollowUser(follower.Id, followee.Id);
                }
                _context.SaveChanges();
                Profile profile = await GetProfile(username, authedUsername);
                transaction.Commit();
                return profile;
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
    }
}
