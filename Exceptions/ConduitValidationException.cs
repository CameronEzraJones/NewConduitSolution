using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Exceptions
{
    public class ConduitValidationException : Exception
    {
        public List<string> Errors;

        public ConduitValidationException(string message) : base(message)
        {
        }

        public ConduitValidationException(List<string> errors)
        {
            this.Errors = errors;
        }
    }
}
