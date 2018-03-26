using Conduit.Model;
using System.Threading.Tasks;

namespace Conduit.Services
{
    public abstract class IUserService
    {
        internal abstract Task<User> AuthenticateUser(string username, string password);
        internal abstract Task<User> RegisterUser(string username, string email, string password);
        internal abstract Task<User> GetCurrentUser(string username);
        internal abstract Task<User> UpdateUser(string username, UserUpdateData user);
    }
}
