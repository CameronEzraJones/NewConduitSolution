using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class NewCommentHolder
    {
        public NewComment Comment { get; set; }

        public NewCommentHolder(NewComment comment)
        {
            Comment = comment;
        }
    }
}
