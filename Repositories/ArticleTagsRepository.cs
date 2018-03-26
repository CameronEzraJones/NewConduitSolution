using Conduit.Context;
using Conduit.Model.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public class ArticleTagsRepository : IArticleTagsRepository
    {

        private readonly DbSet<ArticleTagsDAO> _articleTags;

        public ArticleTagsRepository(ConduitDbContext context)
        {
            _articleTags = context.ArticleTag;
        }

        internal override void AddNewTagForArticle(int id, int tagId)
        {
            ArticleTagsDAO articleTags = new ArticleTagsDAO
            {
                ArticleId = id,
                TagId = tagId
            };
            _articleTags.Add(articleTags);
        }

        internal override List<int> GetArticlesWithTag(int id)
        {
            return _articleTags.Where(e => e.TagId == id).Select(e => e.ArticleId).ToList();
        }

        internal override List<string> GetTagListForArticle(int id)
        {
            return _articleTags.Where(e => e.ArticleId == id).Select(e => e.Tag.Tag).ToList();
        }

        internal override void RemoveTagFromArticle(int id, int tagId)
        {
            ArticleTagsDAO articleTags = _articleTags.Where(e => e.ArticleId == id && e.TagId == tagId).Single();
            _articleTags.Remove(articleTags);
        }

        internal override void SaveArticleTags(List<int> tagList, int id)
        {
            foreach(int tagId in tagList)
            {
                ArticleTagsDAO articleTags = new ArticleTagsDAO
                {
                    ArticleId = id,
                    TagId = tagId
                };
                _articleTags.Add(articleTags);
            }
        }
    }
}
