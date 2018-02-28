using Conduit.Model.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Context
{
    public class ConduitDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<UserPersonalizationDTO> UserPersonalization { get; set; }

        public ConduitDbContext(DbContextOptions<ConduitDbContext> options) : base(options)
        {
        }

        protected ConduitDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserPersonalizationDTO>();
        }
    }
}