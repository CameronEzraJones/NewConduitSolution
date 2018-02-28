using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class UserUpdateDataHolder
    {
        public UserUpdateData User { get; set; }

        public UserUpdateDataHolder(UserUpdateData userUpdateData)
        {
            User = userUpdateData;
        }
    }
}
