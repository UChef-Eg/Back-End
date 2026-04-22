using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UcheifBackend.Models;

namespace UcheifBackend.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CustomerProfile> CustomerProfiles { get; set; }
    public DbSet<CookProfile> CookProfiles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
            .HasOne(u => u.CustomerProfile)
            .WithOne(p => p.User)
            .HasForeignKey<CustomerProfile>(p => p.UserId);

        builder.Entity<AppUser>()
            .HasOne(u => u.CookProfile)
            .WithOne(p => p.User)
            .HasForeignKey<CookProfile>(p => p.UserId);

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
    }
}
