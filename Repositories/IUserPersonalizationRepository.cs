using Conduit.Model.DAO;

namespace Conduit.Repositories
{
    public abstract class IUserPersonalizationRepository
    {
        internal abstract UserPersonalizationDAO GetUserPersonalization(string userId);
        internal abstract void CreateUserPersonalizationForId(string id);
        internal abstract void UpdateUserBio(string id, string bio);
        internal abstract void UpdateUserImage(string id, string image);
    }
}
