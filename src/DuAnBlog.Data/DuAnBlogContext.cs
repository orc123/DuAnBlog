﻿using DuAnBlog.Core.Domain.Content;
using DuAnBlog.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DuAnBlog.Data;
public class DuAnBlogContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DuAnBlogContext(DbContextOptions<DuAnBlogContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<PostActivityLog> PostActivityLogs { get; set; }
    public DbSet<PostCategory> PostCategories { get; set; }
    public DbSet<PostInSeries> PostInSeries { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(x => x.Id);
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims").HasKey(x => x.Id);
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);
        builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(x => new { x.RoleId, x.UserId});
        builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(x => x.UserId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
              .Entries()
              .Where(e => e.State == EntityState.Added);

        foreach (var entityEntry in entries)
        {
            var dateCreatedProp = entityEntry.Entity.GetType().GetProperty("DateCreated");
            if (entityEntry.State == EntityState.Added
                && dateCreatedProp != null)
            {
                dateCreatedProp.SetValue(entityEntry.Entity, DateTime.Now);
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
