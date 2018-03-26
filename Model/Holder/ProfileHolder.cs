using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class ProfileHolder
    {
        public Profile Profile { get; set; }

        public ProfileHolder(Profile profile)
        {
            Profile = profile;
        }
    }
}
