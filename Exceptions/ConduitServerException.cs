using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Exceptions
{
    public class ConduitServerException : Exception
    {
        public ConduitServerException(string message) : base(message)
        {
        }
    }
}
