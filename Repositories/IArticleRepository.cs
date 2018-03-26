using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conduit.Model;
using Conduit.Model.DAO;

namespace Conduit.Repositories
{
    public abstract class IArticleRepository
    {
        internal abstract ArticleDAO CreateArticle(string title, string description, string body, string authorId);
        internal abstract List<ArticleDAO> GetArticlesByArticleIds(List<int> articleIds);
        internal abstract List<ArticleDAO> GetArticlesByAuthorId(String authorId);
        internal abstract List<ArticleDAO> GetArticlesByAuthorIds(List<String> authorIds);
        internal abstract ArticleDAO GetArticleById(int articleId);
        internal abstract Task<ArticleDAO> GetArticleBySlug(string slug);
        internal abstract void UpdateArticle(int articleId, UpdateArticle article);
        internal abstract void DeleteArticle(int articleId);
        internal abstract List<int> GetAllArticleIds();
    }
}
