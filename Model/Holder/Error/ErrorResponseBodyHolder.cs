using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder.Error
{
    public class ErrorResponseBodyHolder
    {
        public List<string> Body { get; set; }

        public ErrorResponseBodyHolder(string error)
        {
            Body = new List<string>
            {
                error
            };
        }

        public ErrorResponseBodyHolder(ICollection<string> errors)
        {
            Body = errors.Cast<string>().ToList();
        }

        public void AddErrorKey(String error)
        {
            if(null == Body)
            {
                Body = new List<string>();
            }
            Body.Add(error);
        }

        public void AddErrorKeys(ICollection<string> errors)
        {
            if(null == Body)
            {
                Body = errors.Cast<string>().ToList();
            } else
            {
                Body.AddRange(errors);
            }
        }
    }
}
