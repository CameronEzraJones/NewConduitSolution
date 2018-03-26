using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Exceptions
{
    public class ConduitUnauthorizedException : Exception
    {
        public ConduitUnauthorizedException(string message) : base(message)
        {
        }
    }
}
