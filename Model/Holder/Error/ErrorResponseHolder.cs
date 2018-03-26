using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder.Error
{
    public class ErrorResponseHolder
    {
        public ErrorResponseBodyHolder Errors { get; set; }
        public String Trace { get; set; }

        public ErrorResponseHolder(string error, String trace)
        {
            Errors = new ErrorResponseBodyHolder(error);
            Trace = trace;
        }

        public ErrorResponseHolder(ICollection<string> errors, String trace)
        {
            Errors = new ErrorResponseBodyHolder(errors);
            Trace = trace;
        }

        public void AddErrorKey(string error)
        {
            if(null == Errors)
            {
                Errors = new ErrorResponseBodyHolder(error);
            } else
            {
                Errors.AddErrorKey(error);
            }
        }

        public void AddErrorKeys(ICollection<string> errors)
        {
            if(null == Errors)
            {
                Errors = new ErrorResponseBodyHolder(errors);
            } else
            {
                Errors.AddErrorKeys(errors);
            }
        }
    }
}
