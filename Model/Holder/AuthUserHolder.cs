using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class AuthUserHolder
    {
        public AuthUser User { get; set; }

        public AuthUserHolder(AuthUser user)
        {
            User = user;
        }
    }
}
