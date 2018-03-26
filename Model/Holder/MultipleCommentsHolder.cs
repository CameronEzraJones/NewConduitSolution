using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class MultipleCommentsHolder
    {
        public List<Comment> Comments;

        public MultipleCommentsHolder(List<Comment> comments)
        {
            Comments = comments;
        }
    }
}
