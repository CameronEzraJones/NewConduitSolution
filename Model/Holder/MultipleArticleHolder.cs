using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class MultipleArticleHolder
    {
        public List<Article> Articles;
        public int ArticlesCount;

        public MultipleArticleHolder(List<Article> articles)
        {
            Articles = articles;
            ArticlesCount = articles.Count;
        }
    }
}
