using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder.Error
{
    public class ErrorResponseHolder
    {
        public ErrorResponseBodyHolder Errors { get; set; }

        public ErrorResponseHolder(string error)
        {
            Errors = new ErrorResponseBodyHolder(error);
        }

        public ErrorResponseHolder(ICollection<string> errors)
        {
            Errors = new ErrorResponseBodyHolder(errors);
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
