using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class TagsHolder
    {
        public List<string> Tags { get; set; }

        public TagsHolder(List<string> tags)
        {
            Tags = tags;
        }
    }
}
