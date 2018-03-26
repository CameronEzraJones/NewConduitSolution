using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class SingleArticleHolder
    {
        public Article Article;

        public SingleArticleHolder(Article article)
        {
            Article = article;
        }
    }
}
