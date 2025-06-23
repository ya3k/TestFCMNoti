using Domain.Entity;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class EPDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public EPDbContext(DbContextOptions options) : base(options)
        {

        }
        public EPDbContext() { }

        public DbSet<FcmDeviceTokens> FcmDeviceTokens { get; set; }
        public DbSet<FcmNotification> FcmNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EPDbContext).Assembly);

            // Seed default roles
            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = ApplicationRole.ADMIN,
                    NormalizedName = ApplicationRole.ADMIN.ToUpper()
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = ApplicationRole.USER,
                    NormalizedName = ApplicationRole.USER.ToUpper()
                }
            );
            var hasher = new PasswordHasher<ApplicationUser>();

            // Seed default admin user
            modelBuilder.Entity<ApplicationUser>().HasData(
               new ApplicationUser
               {
                   Id = Guid.Parse("e4eaaaf2-d142-11e1-b3e4-080027620cdd"),
                   UserName = "admin",
                   PersonName = "Admin User",
                   NormalizedUserName = "ADMIN",
                   Email = "van23@example.com",
                   NormalizedEmail = "VAN23@EXAMPLE.COM",
                   EmailConfirmed = true,
                   PasswordHash = hasher.HashPassword(null, "23012003aA@"),
                   SecurityStamp = Guid.NewGuid().ToString("D"),
                   ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                   IsActive = true,
                   CreatedAt = DateTime.UtcNow,
                   UpdatedAt = DateTime.UtcNow
               }
            );

            //seed user role
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("e4eaaaf2-d142-11e1-b3e4-080027620cdd"),
                    RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111") // Admin role
                }
            );


        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            //configurationBuilder.RegisterAllInVogenEfCoreConverters();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<ApplicationUser>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }


    }

}
