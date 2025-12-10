using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TypeFormProject.DataAccess.Entities;

namespace TypeFormProject.DataAccess.Context
{
        public class AppDbContext : IdentityDbContext<AppUser>
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<Organization> Organizations => Set<Organization>();
            public DbSet<UserOrgRole> UserOrgRoles => Set<UserOrgRole>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

               
                modelBuilder.Entity<Organization>(e =>
                {
                    e.HasKey(x => x.Id);
                    e.Property(x => x.Name).IsRequired().HasMaxLength(150);
                    e.HasIndex(x => x.Name).IsUnique(); 
                    e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                });

               
                modelBuilder.Entity<AppUser>(e =>
                {
                    e.Property(x => x.FullName).IsRequired().HasMaxLength(150);
                    e.Property(x => x.IsActive).HasDefaultValue(true);
                    
                });

                modelBuilder.Entity<UserOrgRole>(e =>
                {
                    e.HasKey(x => x.Id);

                    e.HasIndex(x => new { x.OrganizationId, x.UserId }).IsUnique();

                    
                    e.Property(x => x.Role).IsRequired();

                    e.HasOne(x => x.Organization)
                     .WithMany(o => o.UserRoles)
                     .HasForeignKey(x => x.OrganizationId)
                     .OnDelete(DeleteBehavior.Cascade);

                    e.HasOne(x => x.User)
                     .WithMany(u => u.OrgRoles)
                     .HasForeignKey(x => x.UserId)
                     .OnDelete(DeleteBehavior.Cascade);
                });
            }
        }
    }

