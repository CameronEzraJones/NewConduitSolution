using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Utils
{
    public class LoggingUtil
    {
        public static void LogError(ILogger logger, string message)
        {
            message = message + ", Trace GUID: " + Guid.NewGuid().ToString();
            logger.LogError(message);
        }
    }
}
