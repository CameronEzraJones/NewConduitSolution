using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Repositories
{
    public abstract class IArticleTagsRepository
    {
        internal abstract void SaveArticleTags(List<int> tagList, int id);
        internal abstract List<string> GetTagListForArticle(int id);
        internal abstract List<int> GetArticlesWithTag(int id);
        internal abstract void AddNewTagForArticle(int id, int tagId);
        internal abstract void RemoveTagFromArticle(int id, int tagId);
    }
}
