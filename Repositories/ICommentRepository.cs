using Conduit.Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public abstract class ICommentRepository
    {
        public abstract CommentDAO CreateComment(String body, String authorId, int articleId);
        public abstract List<CommentDAO> GetCommentsForArticle(int articleId);
        public abstract void DeleteComment(int commentId);
    }
}
