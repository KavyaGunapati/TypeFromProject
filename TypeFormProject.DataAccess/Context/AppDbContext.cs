using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TypeFormProject.DataAccess.Entities;

namespace TypeFormProject.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Organization> Organizations { get; set; } = default!;
        public DbSet<UserOrgRole> UserOrgRoles { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // UserOrgRole relationship
            builder.Entity<UserOrgRole>()
                .HasOne(uor => uor.User)
                .WithMany(u => u.OrgRoles)
                .HasForeignKey(uor => uor.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserOrgRole>()
                .HasOne(uor => uor.Organization)
                .WithMany(o => o.UserRoles)
                .HasForeignKey(uor => uor.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<UserOrgRole>()
                .HasIndex(uor => new { uor.UserId, uor.OrganizationId, uor.Role })
                .IsUnique();

            // Optional: prevent a user having multiple records per org (irrespective of role)
            builder.Entity<UserOrgRole>()
                 .HasIndex(uor => new { uor.UserId, uor.OrganizationId })
               .IsUnique();

            // Optional: Organization name unique
            builder.Entity<Organization>()
                .HasIndex(o => o.Name)
                .IsUnique();
            builder.Entity<Organization>()
                .HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<UserOrgRole>()
                .Property(x => x.Role)
                .HasConversion<int>();

        }
    }

}

