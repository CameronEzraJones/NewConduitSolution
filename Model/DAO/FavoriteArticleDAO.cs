using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.DAO
{
    public class FavoriteArticleDAO
    {
        public IdentityUser User { get; set; }

        [ForeignKey("User")]
        public String UserId { get; set; }

        public ArticleDAO Article { get; set; }

        [ForeignKey("Article")]
        public int ArticleId { get; set; }
    }
}
