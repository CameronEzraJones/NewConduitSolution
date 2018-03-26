using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class SingleCommentHolder
    {
        public Comment Comment;

        public SingleCommentHolder(Comment comment)
        {
            Comment = comment;
        }
    }
}
