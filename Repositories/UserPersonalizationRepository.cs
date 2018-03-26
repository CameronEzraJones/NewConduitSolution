using System.Linq;
using Conduit.Context;
using Conduit.Model.DAO;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Repositories
{
    public class UserPersonalizationRepository : IUserPersonalizationRepository
    {
        private readonly DbSet<UserPersonalizationDAO> _userPersonalization;

        public UserPersonalizationRepository(ConduitDbContext context)
        {
            _userPersonalization = context.UserPersonalization;
        }

        internal override void CreateUserPersonalizationForId(string id)
        {
            _userPersonalization.Add(new UserPersonalizationDAO { UserId = id, Bio = null, Image = null });
        }

        internal override UserPersonalizationDAO GetUserPersonalization(string userId)
        {
            return _userPersonalization.Where(e => e.UserId == userId).Single();
        }

        internal override void UpdateUserBio(string id, string bio)
        {
            UserPersonalizationDAO userPersonalizationDTO = _userPersonalization.Where(e => e.UserId == id).Single();
            userPersonalizationDTO.Bio = bio;
        }

        internal override void UpdateUserImage(string id, string image)
        {
            UserPersonalizationDAO userPersonalizationDTO = _userPersonalization.Where(e => e.UserId == id).Single();
            userPersonalizationDTO.Image = image;
        }
    }
}
