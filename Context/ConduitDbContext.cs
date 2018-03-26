using Conduit.Model.DAO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Context
{
    public class ConduitDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<ArticleDAO> Article { get; set; }
        public DbSet<ArticleTagsDAO> ArticleTag { get; set; }
        public DbSet<CommentDAO> Comment { get; set; }
        public DbSet<FavoriteArticleDAO> FavoriteArticle { get; set; }
        public DbSet<TagDAO> Tag { get; set; }
        public DbSet<UserIsFollowingDAO> UserIsFollowing { get; set; }
        public DbSet<UserPersonalizationDAO> UserPersonalization { get; set; }

        public ConduitDbContext(DbContextOptions<ConduitDbContext> options) : base(options)
        {
        }

        protected ConduitDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ArticleDAO>();
            builder.Entity<ArticleTagsDAO>()
                .HasKey(e => new { e.TagId, e.ArticleId });
            builder.Entity<CommentDAO>();
            builder.Entity<FavoriteArticleDAO>()
                .HasKey(e => new { e.UserId, e.ArticleId });
            builder.Entity<TagDAO>()
                .HasIndex(e => new { e.Tag })
                .IsUnique();
            builder.Entity<UserIsFollowingDAO>()
                .HasKey(e => new { e.FollowerId, e.FolloweeId });
            builder.Entity<UserPersonalizationDAO>();
        }
    }
}