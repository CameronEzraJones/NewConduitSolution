using Conduit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Services
{
    public abstract class IArticleService
    {
        public abstract Task<List<Article>> GetArticles(Dictionary<String, String> query, String authedUsername = null);
        public abstract Task<List<Article>> GetFeedArticles(Dictionary<String, String> query, String authedUsername);
        public abstract Task<Article> GetArticle(String authedUsername, String slug);
        public abstract Task<Article> CreateArticle(String authedUsername, NewArticle newArticle);
        public abstract Task<Article> UpdateArticle(String authedUsername, String articleSlug, UpdateArticle updateArticle);
        public abstract Task DeleteArticle(String authedUsername, String slug);
        public abstract Task<Article> FavoriteArticle(String authedUsername, String slug);
        public abstract Task<Article> UnfavoriteArticle(String authedUsername, String slug);
        public abstract Task<List<String>> GetTags();
    }
}
