using Conduit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Services
{
    public abstract class ICommentService
    {
        public abstract Task<Comment> CreateComment(String username, String comment, String articleSlug);
        public abstract Task<List<Comment>> GetComments(String username, String articleSlug);
        public abstract void DeleteComment(int commentId);
    }
}
