using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.Holder
{
    public class NewArticleHolder
    {
        public NewArticle Article { get; set; }

        public NewArticleHolder(NewArticle article)
        {
            Article = article;
        }
    }
}
