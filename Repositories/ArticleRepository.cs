using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Conduit.Context;
using Conduit.Model;
using Conduit.Model.DAO;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly DbSet<ArticleDAO> _articles;

        public ArticleRepository(ConduitDbContext context)
        {
            _articles = context.Article;
        }

        internal override ArticleDAO CreateArticle(string title, string description, string body, string authorId)
        {
            var now = DateTime.Now;
            ArticleDAO article = new ArticleDAO
            {
                Slug = SlugUtil.CreateSlug(title),
                Title = title,
                Description = description,
                Body = body,
                AuthorId = authorId,
                CreatedAt = now,
                UpdatedAt = now
            };
            _articles.Add(article);
            return article;
        }

        internal override void DeleteArticle(int articleId)
        {
            ArticleDAO article = _articles.Where(e => e.Id == articleId).Single();
            _articles.Remove(article);
        }

        internal override List<int> GetAllArticleIds()
        {
            return _articles.Select(e => e.Id).ToList();
        }

        internal override ArticleDAO GetArticleById(int articleId)
        {
            return _articles.Where(e => e.Id == articleId).SingleOrDefault();
        }

        internal override async Task<ArticleDAO> GetArticleBySlug(string slug)
        {
            return await _articles.Where(e => e.Slug == slug).SingleOrDefaultAsync();
        }

        internal override List<ArticleDAO> GetArticlesByArticleIds(List<int> articleIds)
        {
            return _articles.Where(e => articleIds.Contains(e.Id)).OrderByDescending(e => e.CreatedAt).ToList();
        }

        internal override List<ArticleDAO> GetArticlesByAuthorId(string authorId)
        {
            return _articles.Where(e => e.AuthorId == authorId).OrderByDescending(e => e.CreatedAt).ToList();
        }

        internal override List<ArticleDAO> GetArticlesByAuthorIds(List<string> authorIds)
        {
            return _articles.Where(e => authorIds.Contains(e.AuthorId)).OrderByDescending(e => e.CreatedAt).ToList();
        }

        internal override void UpdateArticle(int articleId, UpdateArticle articleUpdates)
        {
            ArticleDAO article = _articles.Where(e => e.Id == articleId).Single();
            if(null != articleUpdates.Title)
            {
                article.Title = articleUpdates.Title;
                article.Slug = SlugUtil.CreateSlug(articleUpdates.Title);
            }
            if(null != articleUpdates.Description)
            {
                article.Description = articleUpdates.Description;
            }
            if(null != articleUpdates.Body)
            {
                article.Body = articleUpdates.Body;
            }
            article.UpdatedAt = DateTime.Now;
            _articles.Update(article);
        }

        public class SlugUtil
        {
            public static String CreateSlug(String title)
            {
                title = RemoveDiacritics(title);
                title = title.ToLower();
                title = Regex.Replace(title, @"[^a-z0-9\s-]", "");
                title = Regex.Replace(title, @"\s+", " ").Trim();
                title = title.Substring(0, title.Length <= 45 ? title.Length : 45).Trim();
                title = Regex.Replace(title, @"\s", "-");
                title = title + "-" + CreateGuidIdentifier();
                return title;
            }

            private static String RemoveDiacritics(String title)
            {
                title = new string(title.Normalize(NormalizationForm.FormD)
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    .ToArray());
                return title.Normalize(NormalizationForm.FormC);
            }

            private static String CreateGuidIdentifier()
            {
                Guid guid = Guid.NewGuid();
                String uniqueString = Convert.ToBase64String(guid.ToByteArray());
                uniqueString = Regex.Replace(uniqueString, @"\+", "");
                uniqueString = Regex.Replace(uniqueString, @"=", "");
                uniqueString = Regex.Replace(uniqueString, @"/", "_");
                return uniqueString;
            }
        }
    }
}
