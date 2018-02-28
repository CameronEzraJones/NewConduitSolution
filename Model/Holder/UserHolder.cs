using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class UserHolder
    {
        public User User { get; set; }

        public UserHolder(User user)
        {
            User = user;
        }
    }
}
