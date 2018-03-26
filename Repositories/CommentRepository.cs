using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Context;
using Conduit.Model.DAO;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Repositories
{
    public class CommentRepository : ICommentRepository
    {

        private readonly DbSet<CommentDAO> _comments;

        public CommentRepository(ConduitDbContext context)
        {
            _comments = context.Comment;
        }

        public override CommentDAO CreateComment(string body, string authorId, int articleId)
        {
            CommentDAO comment = new CommentDAO
            {
                ArticleId = articleId,
                AuthorId = authorId,
                Body = body,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _comments.Add(comment);
            return comment;
        }

        public override void DeleteComment(int commentId)
        {
            CommentDAO comment = _comments.Where(e => e.Id == commentId).Single();
            _comments.Remove(comment);
        }

        public override List<CommentDAO> GetCommentsForArticle(int articleId)
        {
            return _comments.Where(e => e.ArticleId == articleId).ToList();
        }
    }
}
