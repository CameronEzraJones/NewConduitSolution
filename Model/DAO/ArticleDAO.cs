using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conduit.Model.DAO
{
    public class ArticleDAO
    {   
        [Key]
        public int Id { get; set; }

        [Required]
        public string Slug { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public IdentityUser User { get; set; }

        [ForeignKey("User")]
        public string AuthorId { get; set; }
    }
}
