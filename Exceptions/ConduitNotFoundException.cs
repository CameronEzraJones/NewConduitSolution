using System;

namespace Conduit.Exceptions
{
    public class ConduitNotFoundException : Exception
    {
        public ConduitNotFoundException(string message) : base(message)
        {
        }
    }
}
