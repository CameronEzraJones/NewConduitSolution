using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.DAO
{
    public class ArticleTagsDAO
    {
        public TagDAO Tag { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }

        public ArticleDAO Article { get; set; }

        [ForeignKey("Article")]
        public int ArticleId { get; set; }
    }
}
