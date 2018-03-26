using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conduit.Model.DAO
{
    public class CommentDAO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public string Body { get; set; }

        public IdentityUser Author { get; set; }

        [ForeignKey("Author")]
        public string AuthorId;

        public ArticleDAO Article { get; set; }

        [ForeignKey("Article")]
        public int ArticleId { get; set; }
    }
}
